using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Components;
using UACF.UI.Editor.Creation;
using UACF.UI.Editor.UACF;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIElementHandler
    {
        private static readonly System.Collections.Generic.Dictionary<string, string> ComponentToPrefab = new()
        {
            { "UIText", "Display/UIText" },
            { "UIImage", "Display/UIImage" },
            { "UIIcon", "Display/UIIcon" },
            { "UIDivider", "Display/UIDivider" },
            { "UIBadge", "Display/UIBadge" },
            { "UIButton", "Input/UIButton_Filled" },
            { "UIButton_Filled", "Input/UIButton_Filled" },
            { "UIButton_Outlined", "Input/UIButton_Outlined" },
            { "UIButton_Text", "Input/UIButton_Text" },
            { "UIButton_Tonal", "Input/UIButton_Tonal" },
            { "UIIconButton", "Input/UIIconButton" },
            { "UIToggle", "Input/UIToggle" },
            { "UISlider", "Input/UISlider" },
            { "UIDropdown", "Input/UIDropdown" },
            { "UIInputField", "Input/UIInputField" },
            { "UICheckbox", "Input/UICheckbox" },
            { "UIVerticalLayout", "Layout/UIVerticalLayout" },
            { "UIHorizontalLayout", "Layout/UIHorizontalLayout" },
            { "UIGridLayout", "Layout/UIGridLayout" },
            { "UISpacer", "Layout/UISpacer" },
            { "UIPanel", "Containers/UIPanel" },
            { "UICard", "Containers/UICard" },
            { "UIScrollView", "Containers/UIScrollView" },
            { "UIList", "Containers/UIList" },
            { "UIListItem", "Containers/UIListItem" },
            { "UITabContainer", "Containers/UITabContainer" },
            { "UIHeader", "Navigation/UIHeader" },
            { "UIToolbar", "Navigation/UIToolbar" },
            { "UIBottomBar", "Navigation/UIBottomBar" },
            { "UITabBar", "Navigation/UITabBar" },
            { "UISidebar", "Navigation/UISidebar" },
            { "UIOverlay", "Overlay/UIOverlay" },
            { "UIModal", "Overlay/UIModal" },
            { "UIDialog", "Overlay/UIDialog" },
            { "UIToast", "Overlay/UIToast" },
            { "UITooltip", "Overlay/UITooltip" },
            { "UIProgressBar", "Feedback/UIProgressBar" },
            { "UISpinner", "Feedback/UISpinner" },
            { "UIHealthBar", "Feedback/UIHealthBar" }
        };

        internal static IReadOnlyDictionary<string, string> GetComponentToPrefabMap() => ComponentToPrefab;

        public static void Register(RequestRouter router)
        {
            router.Register("POST", "/api/ui/element/add", HandleAddElement);
            router.Register("PUT", "/api/ui/element/modify", HandleModifyElement);
            router.Register("DELETE", "/api/ui/element/remove", HandleRemoveElement);
            router.Register("POST", "/api/ui/element/reorder", HandleReorderElement);
        }

        private static async Task HandleAddElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await ExecuteAddElement(body);
            if (result is System.Collections.Generic.Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && !(bool)success)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        public static System.Threading.Tasks.Task<object> ExecuteAddElement(System.Collections.Generic.Dictionary<string, object> body)
        {
            return MainThreadDispatcher.Enqueue<object>(() =>
            {
                var component = body["component"]?.ToString() ?? "UIButton";
                var name = body["name"]?.ToString() ?? "NewElement";
                var parentRef = body.TryGetValue("parent", out var p) ? p : null;
                var properties = body.TryGetValue("properties", out var prop) ? prop as System.Collections.Generic.Dictionary<string, object> : null;

                var prefabKey = "Input/UIButton_Filled";
                if (component == "UIButton" && properties != null && properties.TryGetValue("variant", out var v))
                {
                    var variant = v?.ToString()?.ToLowerInvariant();
                    prefabKey = variant switch
                    {
                        "outlined" => "Input/UIButton_Outlined",
                        "text" => "Input/UIButton_Text",
                        "tonal" => "Input/UIButton_Tonal",
                        _ => "Input/UIButton_Filled"
                    };
                }
                else if (ComponentToPrefab.TryGetValue(component, out var pk))
                    prefabKey = pk;

                var prefabPath = $"Assets/Resources/UACF_UI/Prefabs/{prefabKey}";

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{prefabPath}.prefab");
                if (prefab == null)
                {
                    return new { success = false, error = $"Prefab not found: {prefabPath}. Run bootstrap first: curl -X POST http://localhost:7890/api/ui/setup/bootstrap" };
                }

                Transform parent = null;
                if (parentRef is System.Collections.Generic.Dictionary<string, object> parentDict)
                {
                    if (parentDict.TryGetValue("instance_id", out var idObj) && idObj != null)
                    {
                        var id = System.Convert.ToInt32(idObj);
#pragma warning disable CS0618
                        var go = EditorUtility.InstanceIDToObject(id) as GameObject;
#pragma warning restore CS0618
                        parent = go?.transform;
                    }
                    else if (parentDict.TryGetValue("name", out var nameObj))
                    {
                        var found = GameObject.Find(nameObj?.ToString());
                        parent = found?.transform;
                    }
                }
                if (parent == null)
                {
                    var canvas = Object.FindFirstObjectByType<Canvas>();
                    parent = canvas != null ? canvas.transform : null;
                }
                if (parent == null)
                {
                    var canvasGo = new GameObject("Canvas");
                    canvasGo.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
                    canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                    parent = canvasGo.transform;
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                instance.name = name;
                Undo.RegisterCreatedObjectUndo(instance, "Create " + name);

                var instanceId = instance.GetInstanceID();
                if (properties != null)
                {
                    ApplyProperties(instance, properties);
                }

                var parentGo = parent != null ? parent.gameObject : null;
                if (parentGo != null && parentGo.GetComponent<LayoutGroup>() != null)
                {
                    ApplyLayoutToChild(instance, component, body);
                }

                if (body.TryGetValue("rect", out var rectObj) && rectObj is Dictionary<string, object> rect)
                {
                    ApplyRectToTransform(instance, rect);
                }

                if (body.TryGetValue("scrollRect", out var scrollRectObj) && scrollRectObj is Dictionary<string, object> scrollRect)
                    ApplyScrollRect(instance, scrollRect);
                if (body.TryGetValue("contentSizeFitter", out var csfObj) && csfObj is Dictionary<string, object> csf)
                    ApplyContentSizeFitter(instance, csf);

                return new
                {
                    success = true,
                    created = new[] { new { name, instance_id = instanceId, component } },
                    total = 1
                };
            });
        }

        private static void ApplyProperties(GameObject go, Dictionary<string, object> props)
        {
            foreach (var kv in props)
            {
                var key = kv.Key;
                var val = kv.Value;
                if (val == null) continue;

                if (TryApplySetMethods(go, key, val))
                    continue;

                if (key == "labelText" || key == "variant")
                {
                    var btn = go.GetComponent<UIButton>();
                    if (btn != null)
                    {
                        if (key == "labelText") btn.SetLabel(val?.ToString() ?? "");
                        else if (key == "variant" && val is string vs)
                        {
                            if (Enum.TryParse<UIButtonVariant>(vs, true, out var v))
                                btn.SetVariant(v);
                        }
                    }
                    continue;
                }

                foreach (var comp in go.GetComponents<Component>())
                {
                    if (comp == null) continue;
                    var t = comp.GetType();
                    var field = t.GetField(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field == null) continue;

                    try
                    {
                        var converted = ConvertValue(val, field.FieldType);
                        if (converted != null && (field.FieldType.IsInstanceOfType(converted) || field.FieldType.IsAssignableFrom(converted.GetType())))
                            field.SetValue(comp, converted);
                    }
                    catch { }
                }
            }
        }

        private static bool TryApplySetMethods(GameObject go, string key, object val)
        {
            var slider = go.GetComponent<UISlider>();
            if (slider != null)
            {
                if (key == "value" && TryParseFloat(val, out var v))
                {
                    slider.SetValue(v);
                    return true;
                }
                if (key == "minValue" && TryParseFloat(val, out var min))
                {
                    var max = slider.GetComponent<Slider>()?.maxValue ?? 1f;
                    slider.SetRange(min, max);
                    return true;
                }
                if (key == "maxValue" && TryParseFloat(val, out var max))
                {
                    var min = slider.GetComponent<Slider>()?.minValue ?? 0f;
                    slider.SetRange(min, max);
                    return true;
                }
            }

            var toggle = go.GetComponent<UIToggle>();
            if (toggle != null && key == "isOn")
            {
                toggle.SetOn(val is bool b ? b : bool.TryParse(val?.ToString(), out var parsed) && parsed);
                return true;
            }

            var dropdown = go.GetComponent<UIDropdown>();
            if (dropdown != null && key == "options")
            {
                var list = ConvertToListOfStrings(val);
                if (list != null)
                {
                    dropdown.SetOptions(list);
                    return true;
                }
            }

            var progressBar = go.GetComponent<UIProgressBar>();
            if (progressBar != null && key == "value" && TryParseFloat(val, out var pv))
            {
                progressBar.SetValue(Mathf.Clamp01(pv));
                return true;
            }

            var healthBar = go.GetComponent<UIHealthBar>();
            if (healthBar != null)
            {
                if (key == "currentHealth" && TryParseFloat(val, out var ch))
                {
                    var max = healthBar.maxHealth;
                    healthBar.SetHealth(ch, max);
                    return true;
                }
                if (key == "maxHealth" && TryParseFloat(val, out var mh))
                {
                    var cur = healthBar.currentHealth;
                    healthBar.SetHealth(cur, mh);
                    return true;
                }
            }

            return false;
        }

        private static object ConvertValue(object val, Type targetType)
        {
            if (val is Dictionary<string, object> dict && dict.TryGetValue("asset", out var pathObj))
            {
                var path = pathObj?.ToString();
                if (string.IsNullOrEmpty(path)) return null;
                if (targetType == typeof(Sprite) || targetType == typeof(UnityEngine.Object))
                    return AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (targetType == typeof(Texture2D))
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (targetType == typeof(Font))
                    return AssetDatabase.LoadAssetAtPath<Font>(path);
#if UNITY_EDITOR
                if (targetType.Name == "TMP_FontAsset")
                {
                    var t = Type.GetType("TMPro.TMP_FontAsset, Unity.TextMeshPro");
                    if (t != null)
                        return AssetDatabase.LoadAssetAtPath(path, t);
                }
#endif
                if (typeof(ScriptableObject).IsAssignableFrom(targetType))
                    return AssetDatabase.LoadAssetAtPath(path, targetType);
                return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }

            if (val is IList list && targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elemType = targetType.GetGenericArguments()[0];
                if (elemType == typeof(string))
                {
                    var result = new List<string>();
                    foreach (var item in list)
                        result.Add(item?.ToString() ?? "");
                    return result;
                }
                if (elemType == typeof(int))
                {
                    var result = new List<int>();
                    foreach (var item in list)
                    {
                        if (item is int i) result.Add(i);
                        else if (item is long l) result.Add((int)l);
                        else if (item != null && int.TryParse(item.ToString(), out var parsed)) result.Add(parsed);
                    }
                    return result;
                }
                if (elemType == typeof(float))
                {
                    var result = new List<float>();
                    foreach (var item in list)
                    {
                        if (item is float f) result.Add(f);
                        else if (item is double d) result.Add((float)d);
                        else if (item is int i) result.Add(i);
                        else if (item is long l) result.Add(l);
                        else if (item != null && float.TryParse(item.ToString(), out var parsed)) result.Add(parsed);
                    }
                    return result;
                }
            }

            if (val is double d && targetType == typeof(float)) return (float)d;
            if (val is long l && targetType == typeof(int)) return (int)l;
            if (val is string s && targetType.IsEnum) return Enum.Parse(targetType, s, true);
            return val;
        }

        private static List<string> ConvertToListOfStrings(object val)
        {
            if (val is List<string> ls) return ls;
            if (val is IList list)
            {
                var result = new List<string>();
                foreach (var item in list)
                    result.Add(item?.ToString() ?? "");
                return result;
            }
            return null;
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

        public static Task<object> ExecuteModifyElement(Dictionary<string, object> body)
        {
            return MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (!body.TryGetValue("target", out var targetObj) || targetObj is not Dictionary<string, object> target)
                    return new { success = false, error = "target required" };

                var go = ResolveTarget(target);
                if (go == null)
                    return new { success = false, error = "Target not found" };

                if (body.TryGetValue("set", out var setObj) && setObj is Dictionary<string, object> set)
                {
                    if (set.TryGetValue("name", out var nameVal) && nameVal != null)
                        go.name = nameVal.ToString();
                    if (set.TryGetValue("properties", out var propsObj) && propsObj is Dictionary<string, object> props)
                        ApplyProperties(go, props);
                    if (set.TryGetValue("layout", out var layoutObj) && layoutObj is Dictionary<string, object> layout)
                    {
                        var le = go.GetComponent<LayoutElement>();
                        if (le == null) le = go.AddComponent<LayoutElement>();
                        ApplyLayoutFromDict(le, layout);
                    }
                }

                if (body.TryGetValue("rect", out var rectObj) && rectObj is Dictionary<string, object> rect)
                {
                    var rt = go.GetComponent<RectTransform>();
                    if (rt != null)
                    {
                        if (rect.TryGetValue("anchorMin", out var am) && am is Dictionary<string, object> amd)
                            rt.anchorMin = ParseVector2(amd);
                        if (rect.TryGetValue("anchorMax", out var ax) && ax is Dictionary<string, object> axd)
                            rt.anchorMax = ParseVector2(axd);
                        if (rect.TryGetValue("offsetMin", out var om) && om is Dictionary<string, object> omd)
                            rt.offsetMin = ParseVector2(omd);
                        if (rect.TryGetValue("offsetMax", out var ox) && ox is Dictionary<string, object> oxd)
                            rt.offsetMax = ParseVector2(oxd);
                        if (rect.TryGetValue("sizeDelta", out var sd) && sd is Dictionary<string, object> sdd)
                            rt.sizeDelta = ParseVector2(sdd);
                    }
                }

                if (body.TryGetValue("scrollRect", out var scrollRectObj) && scrollRectObj is Dictionary<string, object> scrollRect)
                    ApplyScrollRect(go, scrollRect);
                if (body.TryGetValue("contentSizeFitter", out var csfObj) && csfObj is Dictionary<string, object> csf)
                    ApplyContentSizeFitter(go, csf);

                EditorUtility.SetDirty(go);
                return new { success = true, updated = true };
            });
        }

        private static void ApplyScrollRect(GameObject go, Dictionary<string, object> scrollRect)
        {
            var sr = go.GetComponent<ScrollRect>();
            if (sr == null) return;

            if (scrollRect.TryGetValue("horizontal", out var h) && h is bool hb)
                sr.horizontal = hb;
            if (scrollRect.TryGetValue("vertical", out var v) && v is bool vb)
                sr.vertical = vb;
            if (scrollRect.TryGetValue("movementType", out var mt) && mt != null)
            {
                if (Enum.TryParse<ScrollRect.MovementType>(mt.ToString(), true, out var mtv))
                    sr.movementType = mtv;
            }
            if (scrollRect.TryGetValue("elasticity", out var el) && TryParseFloat(el, out var elv))
                sr.elasticity = elv;
            if (scrollRect.TryGetValue("scrollSensitivity", out var ss) && TryParseFloat(ss, out var ssv))
                sr.scrollSensitivity = ssv;
        }

        private static void ApplyContentSizeFitter(GameObject go, Dictionary<string, object> csf)
        {
            GameObject content = null;
            var scrollView = go.GetComponent<UIScrollView>();
            if (scrollView != null)
                content = scrollView.GetContent()?.gameObject;
            if (content == null)
                content = go.transform.Find("Viewport/Content")?.gameObject ?? go.transform.Find("Content")?.gameObject;
            if (content == null)
                content = go;

            var fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = content.AddComponent<ContentSizeFitter>();

            if (csf.TryGetValue("horizontalFit", out var hf) && hf != null)
            {
                if (Enum.TryParse<ContentSizeFitter.FitMode>(hf.ToString(), true, out var hfv))
                    fitter.horizontalFit = hfv;
            }
            if (csf.TryGetValue("verticalFit", out var vf) && vf != null)
            {
                if (Enum.TryParse<ContentSizeFitter.FitMode>(vf.ToString(), true, out var vfv))
                    fitter.verticalFit = vfv;
            }
        }

        private static async Task HandleModifyElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await ExecuteModifyElement(body);

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleRemoveElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (!body.TryGetValue("target", out var targetObj) || targetObj is not Dictionary<string, object> target)
                    return new { success = false, error = "target required" };

                var go = ResolveTarget(target);
                if (go == null)
                    return new { success = false, error = "Target not found" };

                Undo.DestroyObjectImmediate(go);
                return new { success = true, removed = true };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static async Task HandleReorderElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                if (!body.TryGetValue("target", out var targetObj) || targetObj is not Dictionary<string, object> target)
                    return new { success = false, error = "target required" };

                if (!body.TryGetValue("sibling_index", out var idxObj))
                    return new { success = false, error = "sibling_index required" };

                var idx = System.Convert.ToInt32(idxObj);
                var go = ResolveTarget(target);
                if (go == null)
                    return new { success = false, error = "Target not found" };

                go.transform.SetSiblingIndex(idx);
                return new { success = true, reordered = true };
            });

            if (result is Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && success is bool b && !b)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        internal static GameObject ResolveTarget(Dictionary<string, object> target)
        {
            if (target.TryGetValue("instance_id", out var idObj) && idObj != null)
            {
                var id = System.Convert.ToInt32(idObj);
#pragma warning disable CS0618
                return EditorUtility.InstanceIDToObject(id) as GameObject;
#pragma warning restore CS0618
            }
            if (target.TryGetValue("name", out var nameObj))
            {
                var found = GameObject.Find(nameObj?.ToString());
                return found;
            }
            if (target.TryGetValue("path", out var pathObj))
            {
                var path = pathObj?.ToString();
                if (string.IsNullOrEmpty(path)) return null;
                var canvas = Object.FindFirstObjectByType<Canvas>();
                if (canvas == null) return null;
                var t = canvas.transform.Find(path);
                return t != null ? t.gameObject : null;
            }
            return null;
        }

        private static Vector2 ParseVector2(Dictionary<string, object> d)
        {
            return new Vector2(
                d.TryGetValue("x", out var x) ? System.Convert.ToSingle(x) : 0,
                d.TryGetValue("y", out var y) ? System.Convert.ToSingle(y) : 0
            );
        }

        private static void ApplyLayoutToChild(GameObject go, string componentType, Dictionary<string, object> body)
        {
            var le = LayoutElementDefaults.GetOrAdd(go);
            LayoutElementDefaults.ApplyTo(le, componentType);

            if (body.TryGetValue("layout", out var layoutObj) && layoutObj is Dictionary<string, object> layout)
            {
                ApplyLayoutFromDict(le, layout);
            }
        }

        private static void ApplyLayoutFromDict(LayoutElement le, Dictionary<string, object> layout)
        {
            if (layout.TryGetValue("preferredWidth", out var pw) && TryParseFloat(pw, out var pwv) && pwv >= 0)
                le.preferredWidth = pwv;
            if (layout.TryGetValue("preferredHeight", out var ph) && TryParseFloat(ph, out var phv) && phv >= 0)
                le.preferredHeight = phv;
            if (layout.TryGetValue("flexibleWidth", out var fw) && TryParseFloat(fw, out var fwv) && fwv >= 0)
                le.flexibleWidth = fwv;
            if (layout.TryGetValue("flexibleHeight", out var fh) && TryParseFloat(fh, out var fhv) && fhv >= 0)
                le.flexibleHeight = fhv;
            if (layout.TryGetValue("minWidth", out var mw) && TryParseFloat(mw, out var mwv) && mwv >= 0)
                le.minWidth = mwv;
            if (layout.TryGetValue("minHeight", out var mh) && TryParseFloat(mh, out var mhv) && mhv >= 0)
                le.minHeight = mhv;
            if (layout.TryGetValue("layoutPriority", out var lp) && int.TryParse(lp?.ToString(), out var lpv))
                le.layoutPriority = lpv;
        }

        private static void ApplyRectToTransform(GameObject go, Dictionary<string, object> rect)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) return;

            if (rect.TryGetValue("anchorMin", out var am) && am is Dictionary<string, object> amd)
                rt.anchorMin = ParseVector2(amd);
            if (rect.TryGetValue("anchorMax", out var ax) && ax is Dictionary<string, object> axd)
                rt.anchorMax = ParseVector2(axd);
            if (rect.TryGetValue("offsetMin", out var om) && om is Dictionary<string, object> omd)
                rt.offsetMin = ParseVector2(omd);
            if (rect.TryGetValue("offsetMax", out var ox) && ox is Dictionary<string, object> oxd)
                rt.offsetMax = ParseVector2(oxd);
            if (rect.TryGetValue("sizeDelta", out var sd) && sd is Dictionary<string, object> sdd)
                rt.sizeDelta = ParseVector2(sdd);
        }
    }
}
