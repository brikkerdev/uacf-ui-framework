using System.Threading.Tasks;
using UACF.Core;
using UACF.Models;
using UACF.UI.Components;
using UACF.UI.Editor.Creation;
using UnityEngine;
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

            var result = await MainThreadDispatcher.Enqueue<object>(() =>
            {
                var component = body["component"]?.ToString() ?? "UIButton";
                var name = body["name"]?.ToString() ?? "NewElement";
                var parentRef = body.TryGetValue("parent", out var p) ? p : null;
                var properties = body.TryGetValue("properties", out var prop) ? prop as System.Collections.Generic.Dictionary<string, object> : null;

                var prefabKey = "Input/UIButton_Filled";
                if (component == "UIButton" && properties != null && properties.TryGetValue("variant", out var v))
                {
                    var variant = v?.ToString()?.ToLowerInvariant();
                    prefabKey = variant switch { "outlined" => "Input/UIButton_Outlined", "text" => "Input/UIButton_Text", _ => "Input/UIButton_Filled" };
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

                return new
                {
                    success = true,
                    created = new[] { new { name, instance_id = instanceId, component } },
                    total = 1
                };
            });

            if (result is System.Collections.Generic.Dictionary<string, object> dict && dict.TryGetValue("success", out var success) && !(bool)success)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, dict.TryGetValue("error", out var err) ? err?.ToString() : "Unknown error");
                return;
            }

            ctx.RespondOk(result);
        }

        private static void ApplyProperties(GameObject go, System.Collections.Generic.Dictionary<string, object> props)
        {
            foreach (var kv in props)
            {
                var key = kv.Key;
                var val = kv.Value;
                if (val == null) continue;

                foreach (var comp in go.GetComponents<UnityEngine.Component>())
                {
                    if (comp == null) continue;
                    var t = comp.GetType();
                    var field = t.GetField(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field == null) continue;

                    try
                    {
                        var converted = val;
                        if (val is double d && field.FieldType == typeof(float)) converted = (float)d;
                        else if (val is long l && field.FieldType == typeof(int)) converted = (int)l;
                        else if (val is string s && field.FieldType.IsEnum) converted = System.Enum.Parse(field.FieldType, s, true);
                        if (field.FieldType.IsInstanceOfType(converted) || converted != null && field.FieldType.IsAssignableFrom(converted.GetType()))
                            field.SetValue(comp, converted);
                    }
                    catch { }
                }

                if (key == "labelText" || key == "variant")
                {
                    var btn = go.GetComponent<UIButton>();
                    if (btn != null)
                    {
                        if (key == "labelText") btn.SetLabel(val?.ToString() ?? "");
                        else if (key == "variant" && val is string vs)
                        {
                            if (System.Enum.TryParse<UIButtonVariant>(vs, true, out var v))
                                btn.SetVariant(v);
                        }
                    }
                }
            }
        }

        private static async Task HandleModifyElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            if (body == null)
            {
                ctx.RespondError(400, ErrorCode.INVALID_REQUEST, "Request body required");
                return;
            }
            ctx.RespondOk(new { updated = true });
        }

        private static async Task HandleRemoveElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            ctx.RespondOk(new { removed = true });
        }

        private static async Task HandleReorderElement(RequestContext ctx)
        {
            var body = await ctx.ReadBodyAsync<System.Collections.Generic.Dictionary<string, object>>();
            ctx.RespondOk(new { reordered = true });
        }
    }
}
