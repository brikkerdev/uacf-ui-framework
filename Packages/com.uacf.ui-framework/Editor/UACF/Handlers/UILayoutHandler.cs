using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Editor.Creation;
using UACF.UI.Tokens;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UILayoutHandler
    {
        private static readonly Dictionary<string, string> TypeToPrefab = new()
        {
            { "vertical", "Layout/UIVerticalLayout" },
            { "horizontal", "Layout/UIHorizontalLayout" },
            { "grid", "Layout/UIGridLayout" }
        };

        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/layout/create", HandleCreateLayout);
        }

        private static async Task HandleCreateLayout(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await ExecuteCreateLayout(body);
            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        public static System.Threading.Tasks.Task<object> ExecuteCreateLayout(Dictionary<string, object> body)
        {
            return MainThreadDispatcher.Enqueue<object>(() =>
            {
                var type = body.TryGetValue("type", out var t) ? t?.ToString()?.ToLowerInvariant() ?? "vertical" : "vertical";
                var name = body.TryGetValue("name", out var n) ? n?.ToString() ?? "NewLayout" : "NewLayout";
                var parentRef = body.TryGetValue("parent", out var p) ? p : null;

                if (!TypeToPrefab.TryGetValue(type, out var prefabKey))
                    prefabKey = "Layout/UIVerticalLayout";

                var prefabPath = $"{PrefabFactory.PrefabsPath}/{prefabKey}.prefab";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                    return new { success = false, error = $"Layout prefab not found: {prefabPath}. Run bootstrap first." };

                Transform parent = ResolveParent(parentRef);
                if (parent == null)
                    return new { success = false, error = "Could not resolve parent. Provide parent with name, instance_id, or ensure Canvas exists." };

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                instance.name = name;
                Undo.RegisterCreatedObjectUndo(instance, "Create " + name);

                ApplyLayoutParams(instance, type, body);

                return new { success = true, instance_id = instance.GetInstanceID(), type, name };
            });
        }

        private static Transform ResolveParent(object parentRef)
        {
            if (parentRef is Dictionary<string, object> parentDict)
            {
                if (parentDict.TryGetValue("instance_id", out var idObj) && idObj != null)
                {
                    var id = System.Convert.ToInt32(idObj);
#pragma warning disable CS0618
                    var go = EditorUtility.InstanceIDToObject(id) as GameObject;
#pragma warning restore CS0618
                    return go?.transform;
                }
                if (parentDict.TryGetValue("name", out var nameObj))
                {
                    var found = GameObject.Find(nameObj?.ToString());
                    return found?.transform;
                }
            }

            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas != null) return canvas.transform;

            var canvasGo = new GameObject("Canvas");
            canvasGo.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvasGo.transform;
        }

        private static void ApplyLayoutParams(GameObject instance, string type, Dictionary<string, object> body)
        {
            var spacingScale = AssetDatabase.LoadAssetAtPath<SpacingScale>($"{PrefabFactory.TokensPath}/DefaultSpacing.asset");

            var vlg = instance.GetComponent<VerticalLayoutGroup>();
            if (type == "vertical" && vlg != null)
            {
                if (body.TryGetValue("spacing", out var sp))
                {
                    if (TryParseFloat(sp, out var spVal))
                        vlg.spacing = spVal;
                    else if (spacingScale != null && sp is string token)
                        vlg.spacing = spacingScale.GetSpacing(token);
                }
                if (body.TryGetValue("padding", out var pad))
                    vlg.padding = ParsePadding(pad, spacingScale);
                if (body.TryGetValue("childForceExpandWidth", out var cfew))
                    vlg.childForceExpandWidth = cfew is bool b ? b : bool.Parse(cfew?.ToString() ?? "true");
                if (body.TryGetValue("childForceExpandHeight", out var cfeh))
                    vlg.childForceExpandHeight = cfeh is bool b2 ? b2 : bool.Parse(cfeh?.ToString() ?? "false");
                if (body.TryGetValue("childAlignment", out var ca) && ca is string caStr && System.Enum.TryParse<TextAnchor>(caStr, true, out var caVal))
                    vlg.childAlignment = caVal;
            }
            var hlg = instance.GetComponent<HorizontalLayoutGroup>();
            if (type == "horizontal" && hlg != null)
            {
                if (body.TryGetValue("spacing", out var sp))
                {
                    if (TryParseFloat(sp, out var spVal))
                        hlg.spacing = spVal;
                    else if (spacingScale != null && sp is string token)
                        hlg.spacing = spacingScale.GetSpacing(token);
                }
                if (body.TryGetValue("padding", out var pad))
                    hlg.padding = ParsePadding(pad, spacingScale);
                if (body.TryGetValue("childForceExpandWidth", out var cfew))
                    hlg.childForceExpandWidth = cfew is bool b ? b : bool.Parse(cfew?.ToString() ?? "false");
                if (body.TryGetValue("childForceExpandHeight", out var cfeh))
                    hlg.childForceExpandHeight = cfeh is bool b2 ? b2 : bool.Parse(cfeh?.ToString() ?? "true");
                if (body.TryGetValue("childAlignment", out var ca) && ca is string caStr && System.Enum.TryParse<TextAnchor>(caStr, true, out var caVal))
                    hlg.childAlignment = caVal;
            }
            var glg = instance.GetComponent<GridLayoutGroup>();
            if (type == "grid" && glg != null)
            {
                if (body.TryGetValue("spacing", out var sp))
                {
                    if (TryParseFloat(sp, out var spVal))
                        glg.spacing = new Vector2(spVal, spVal);
                    else if (spacingScale != null && sp is string token)
                    {
                        var s = spacingScale.GetSpacing(token);
                        glg.spacing = new Vector2(s, s);
                    }
                }
                if (body.TryGetValue("padding", out var pad))
                    glg.padding = ParsePadding(pad, spacingScale);
                if (body.TryGetValue("cellSize", out var cs) && cs is Dictionary<string, object> csd)
                    glg.cellSize = ParseVector2(csd);
                if (body.TryGetValue("constraint", out var c) && c is string cStr && System.Enum.TryParse<GridLayoutGroup.Constraint>(cStr, true, out var cVal))
                    glg.constraint = cVal;
                if (body.TryGetValue("constraintCount", out var cc) && TryParseInt(cc, out var ccVal))
                    glg.constraintCount = ccVal;
            }
        }

        private static RectOffset ParsePadding(object pad, SpacingScale spacingScale)
        {
            if (pad is double pd)
                return new RectOffset((int)pd, (int)pd, (int)pd, (int)pd);
            if (pad is long pl)
                return new RectOffset((int)pl, (int)pl, (int)pl, (int)pl);
            if (pad is Dictionary<string, object> padDict)
            {
                var left = padDict.TryGetValue("left", out var lv) ? ParseInt(lv, spacingScale) : 0;
                var right = padDict.TryGetValue("right", out var rv) ? ParseInt(rv, spacingScale) : 0;
                var top = padDict.TryGetValue("top", out var tv) ? ParseInt(tv, spacingScale) : 0;
                var bottom = padDict.TryGetValue("bottom", out var bv) ? ParseInt(bv, spacingScale) : 0;
                return new RectOffset(left, right, top, bottom);
            }
            return new RectOffset(0, 0, 0, 0);
        }

        private static int ParseInt(object obj, SpacingScale spacingScale)
        {
            if (obj is double d) return (int)d;
            if (obj is long l) return (int)l;
            if (obj is int i) return i;
            if (obj is string s && spacingScale != null)
                return (int)spacingScale.GetSpacing(s);
            return int.TryParse(obj?.ToString(), out var v) ? v : 0;
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

        private static bool TryParseInt(object obj, out int result)
        {
            result = 0;
            if (obj == null) return false;
            if (obj is double d) { result = (int)d; return true; }
            if (obj is long l) { result = (int)l; return true; }
            if (obj is int i) { result = i; return true; }
            return int.TryParse(obj?.ToString(), out result);
        }

        private static Vector2 ParseVector2(Dictionary<string, object> d)
        {
            return new Vector2(
                d.TryGetValue("x", out var x) ? System.Convert.ToSingle(x) : 0,
                d.TryGetValue("y", out var y) ? System.Convert.ToSingle(y) : 0
            );
        }
    }
}
