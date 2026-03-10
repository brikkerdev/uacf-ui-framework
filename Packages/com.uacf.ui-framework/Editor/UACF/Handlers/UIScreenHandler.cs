using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Editor.Creation;
using UACF.UI.Screens;
using UACF.UI.Theming;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIScreenHandler
    {
        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/screen/create", HandleCreateScreen);
            router.Register("GET", "/api/ui/screen/hierarchy", HandleGetHierarchy);
            router.Register("POST", "/api/ui/screen/show", HandleScreenShow);
            router.Register("POST", "/api/ui/screen/from-spec", HandleScreenFromSpec);
            router.Register("PUT", "/api/ui/canvas/configure", HandleConfigureCanvas);
        }

        private static async Task HandleCreateScreen(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await ExecuteCreateScreen(body);
            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        public static Task<object> ExecuteCreateScreen(Dictionary<string, object> body)
        {
            return MainThreadDispatcher.Enqueue<object>(() =>
            {
                var screenId = body.TryGetValue("screen_id", out var sid) ? sid?.ToString() ?? "main" : "main";
                var name = body.TryGetValue("name", out var n) ? n?.ToString() ?? "MainScreen" : "MainScreen";
                var scenePath = body.TryGetValue("scene_path", out var sp) ? sp?.ToString() : null;

                if (!string.IsNullOrEmpty(scenePath))
                {
                    var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    if (!scene.IsValid())
                        return new { success = false, error = $"Failed to open scene: {scenePath}" };
                }

                var canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    var canvasGo = new GameObject("Canvas");
                    canvas = canvasGo.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                    canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }

                if (body.TryGetValue("canvas", out var canvasObj) && canvasObj is Dictionary<string, object> canvasParams)
                    ApplyCanvasParams(canvas, canvasParams);

                if (canvas.GetComponent<ThemeApplier>() == null)
                    canvas.gameObject.AddComponent<ThemeApplier>();

                var prefabPath = $"{PrefabFactory.PrefabsPath}/Containers/UIPanel.prefab";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                    return new { success = false, error = $"UIPanel prefab not found: {prefabPath}. Run bootstrap first." };

                var root = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
                root.name = name;

                var rt = root.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }

                var marker = root.GetComponent<UIScreenMarker>();
                if (marker == null) marker = root.AddComponent<UIScreenMarker>();
                marker.SetScreenId(screenId);

                Undo.RegisterCreatedObjectUndo(root, "Create " + name);

                var rootId = root.GetInstanceID();
                var contentSlotId = 0;
                var content = root.transform.Find("Content");
                if (content != null)
                    contentSlotId = content.gameObject.GetInstanceID();

                return new
                {
                    success = true,
                    instance_id = rootId,
                    screen_id = screenId,
                    name,
                    root_id = rootId,
                    content_slot_id = contentSlotId
                };
            });
        }

        private static async Task HandleGetHierarchy(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var screenId = ctx.QueryParams.TryGetValue("screen_id", out var sid) ? sid : "";
                var canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null)
                    return new { screen_id = screenId, instance_id = 0, elements = new object[0] };

                var elements = new List<object>();
                CollectHierarchy(canvas.transform, elements, 0);
                return new { screen_id = screenId, instance_id = canvas.GetInstanceID(), elements };
            });
            ctx.RespondOk(data);
        }

        private static void CollectHierarchy(Transform t, List<object> elements, int depth)
        {
            elements.Add(new { name = t.name, instance_id = t.gameObject.GetInstanceID(), depth });
            for (var i = 0; i < t.childCount; i++)
                CollectHierarchy(t.GetChild(i), elements, depth + 1);
        }

        private static async Task HandleScreenShow(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (!body.TryGetValue("screen_id", out var sidObj) || string.IsNullOrEmpty(sidObj?.ToString()))
                    return new { success = false, error = "screen_id required" };

                var targetScreenId = sidObj.ToString();
                var hideOthers = !body.TryGetValue("hide_others", out var ho) || ho is bool hb && hb;

                var canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null)
                    return new { success = false, error = "Canvas not found" };

                var shown = false;
                for (var i = 0; i < canvas.transform.childCount; i++)
                {
                    var child = canvas.transform.GetChild(i).gameObject;
                    var marker = child.GetComponent<UIScreenMarker>();
                    if (marker == null) continue;

                    var shouldShow = string.Equals(marker.ScreenId, targetScreenId, StringComparison.OrdinalIgnoreCase);
                    if (shouldShow)
                    {
                        child.SetActive(true);
                        shown = true;
                    }
                    else if (hideOthers)
                    {
                        child.SetActive(false);
                    }
                }

                EditorUtility.SetDirty(canvas.gameObject);
                return new { success = true, shown, screen_id = targetScreenId };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleScreenFromSpec(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await ExecuteScreenFromSpec(body);

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        public static async Task<object> ExecuteScreenFromSpec(Dictionary<string, object> body)
        {
            var scenePath = body.TryGetValue("scene_path", out var sp) ? sp?.ToString() : null;
            var screenId = body.TryGetValue("screen_id", out var sid) ? sid?.ToString() ?? "main" : "main";
            var name = body.TryGetValue("name", out var n) ? n?.ToString() ?? "MainScreen" : "MainScreen";
            var themeId = body.TryGetValue("theme_id", out var tid) ? tid?.ToString() : null;
            var canvasObj = body.TryGetValue("canvas", out var co) ? co as Dictionary<string, object> : null;

            var screenCreateBody = new Dictionary<string, object>
            {
                ["screen_id"] = screenId,
                ["name"] = name
            };
            if (!string.IsNullOrEmpty(scenePath))
                screenCreateBody["scene_path"] = scenePath;
            if (canvasObj != null)
                screenCreateBody["canvas"] = canvasObj;

            var screenResult = await ExecuteCreateScreen(screenCreateBody);
            if (screenResult is Dictionary<string, object> sr)
            {
                if (sr.TryGetValue("success", out var succ) && succ is bool sb && !sb)
                    return screenResult;
            }
            else
            {
                return new { success = false, error = "Unexpected screen/create result" };
            }

            var srDict = (Dictionary<string, object>)screenResult;
            var results = new Dictionary<string, object>();
            var rootId = srDict.TryGetValue("instance_id", out var rid) ? rid : null;
            var contentSlotId = srDict.TryGetValue("content_slot_id", out var cid) ? cid : null;
            if (rootId != null)
                results["root"] = new Dictionary<string, object> { ["instance_id"] = rootId };
            if (contentSlotId != null)
                results["content"] = new Dictionary<string, object> { ["instance_id"] = contentSlotId };

            var resultsList = new List<object> { new { id = "root", result = screenResult, instance_id = rootId } };

            if (body.TryGetValue("operations", out var opsObj) && (opsObj is System.Collections.IList || opsObj is object[]))
            {
                var opsList = opsObj as System.Collections.IList ?? (opsObj as object[]);
                foreach (var opObj in opsList)
                {
                    if (opObj is not Dictionary<string, object> op)
                        continue;

                    var id = op.TryGetValue("id", out var idVal) ? idVal?.ToString() : null;
                    var method = op.TryGetValue("method", out var m) ? m?.ToString()?.ToUpperInvariant() ?? "POST" : "POST";
                    var path = op.TryGetValue("path", out var p) ? p?.ToString() : null;
                    var opBody = op.TryGetValue("body", out var ob) ? ob as Dictionary<string, object> : null;

                    if (string.IsNullOrEmpty(path) || opBody == null)
                        continue;

                    var mergedBody = UIBatchHandler.ResolveRefs(new Dictionary<string, object>(opBody), results);
                    object opResult;
                    try
                    {
                        opResult = await UIBatchHandler.Dispatch(method, path, mergedBody);
                    }
                    catch (System.Exception ex)
                    {
                        return new { success = false, error = ex.Message };
                    }

                    var instanceId = UIBatchHandler.ExtractInstanceId(opResult);
                    if (!string.IsNullOrEmpty(id) && instanceId.HasValue)
                        results[id] = new Dictionary<string, object> { ["instance_id"] = instanceId.Value };

                    resultsList.Add(new { id, result = opResult, instance_id = instanceId });
                }
            }

            if (!string.IsNullOrEmpty(themeId))
            {
                var themeResult = await UIThemeHandler.ExecuteApplyTheme(new Dictionary<string, object> { ["theme_id"] = themeId });
                resultsList.Add(new { id = "theme", result = themeResult });
            }

            return new
            {
                success = true,
                screen_id = screenId,
                root_id = rootId,
                results = resultsList
            };
        }

        private static async Task HandleConfigureCanvas(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                Canvas canvas = null;
                if (body.TryGetValue("target", out var targetObj) && targetObj is Dictionary<string, object> target)
                {
                    var go = UIElementHandler.ResolveTarget(target);
                    canvas = go?.GetComponent<Canvas>();
                }
                if (canvas == null)
                    canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null)
                    return new { success = false, error = "Canvas not found" };

                if (body.TryGetValue("renderMode", out var rm) && rm != null)
                {
                    if (Enum.TryParse<RenderMode>(rm.ToString(), true, out var rmv))
                        canvas.renderMode = rmv;
                }
                if (body.TryGetValue("referenceResolution", out var rr) && rr is Dictionary<string, object> rrd)
                {
                    var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                    if (scaler != null)
                    {
                        var x = rrd.TryGetValue("x", out var xo) ? Convert.ToSingle(xo) : scaler.referenceResolution.x;
                        var y = rrd.TryGetValue("y", out var yo) ? Convert.ToSingle(yo) : scaler.referenceResolution.y;
                        scaler.referenceResolution = new Vector2(x, y);
                    }
                }
                if (body.TryGetValue("matchWidthOrHeight", out var mwoh) && TryParseFloat(mwoh, out var mwohv))
                {
                    var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                    if (scaler != null)
                        scaler.matchWidthOrHeight = Mathf.Clamp01(mwohv);
                }
                if (body.TryGetValue("scaleFactor", out var sf) && TryParseFloat(sf, out var sfv))
                {
                    var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                    if (scaler != null)
                        scaler.scaleFactor = Mathf.Max(0.01f, sfv);
                }

                EditorUtility.SetDirty(canvas.gameObject);
                return new { success = true, configured = true };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static void ApplyCanvasParams(Canvas canvas, Dictionary<string, object> canvasParams)
        {
            if (canvasParams.TryGetValue("renderMode", out var rm) && rm != null)
            {
                if (Enum.TryParse<RenderMode>(rm.ToString(), true, out var rmv))
                    canvas.renderMode = rmv;
            }
            var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler != null)
            {
                if (canvasParams.TryGetValue("referenceResolution", out var rr) && rr is Dictionary<string, object> rrd)
                {
                    var x = rrd.TryGetValue("x", out var xo) ? Convert.ToSingle(xo) : 1920;
                    var y = rrd.TryGetValue("y", out var yo) ? Convert.ToSingle(yo) : 1080;
                    scaler.referenceResolution = new Vector2(x, y);
                }
                if (canvasParams.TryGetValue("matchWidthOrHeight", out var mwoh) && TryParseFloat(mwoh, out var mwohv))
                    scaler.matchWidthOrHeight = Mathf.Clamp01(mwohv);
                if (canvasParams.TryGetValue("scaleFactor", out var sf) && TryParseFloat(sf, out var sfv))
                    scaler.scaleFactor = Mathf.Max(0.01f, sfv);
            }
        }

        private static bool TryParseFloat(object obj, out float result)
        {
            result = 0;
            if (obj == null) return false;
            if (obj is double d) { result = (float)d; return true; }
            if (obj is float f) { result = f; return true; }
            if (obj is long l) { result = l; return true; }
            if (obj is int i) { result = i; return true; }
            return float.TryParse(obj?.ToString(), out result);
        }
    }
}
