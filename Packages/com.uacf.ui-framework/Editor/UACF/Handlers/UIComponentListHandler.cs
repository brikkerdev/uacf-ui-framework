using System.Collections.Generic;
using System.Threading.Tasks;
using UACF.Core;

namespace UACF.UI.Editor.UACF.Handlers
{
    public static class UIComponentListHandler
    {
        private static readonly Dictionary<string, string> CategoryMap = new()
        {
            { "Display", "display" },
            { "Input", "input" },
            { "Layout", "layout" },
            { "Containers", "container" },
            { "Navigation", "navigation" },
            { "Overlay", "overlay" },
            { "Feedback", "feedback" }
        };

        private static readonly Dictionary<string, object[]> ComponentProperties = new()
        {
            { "UIText", new object[] { "text", "fontSize", "alignment", "color" } },
            { "UIImage", new object[] { "sprite", "color", "preserveAspect", "raycastTarget" } },
            { "UIIcon", new object[] { "sprite", "color", "iconSize" } },
            { "UIDivider", new object[] { "orientation", "thickness" } },
            { "UIBadge", new object[] { "value", "labelText", "variant" } },
            { "UIButton", new object[] { "labelText", "variant" } },
            { "UIIconButton", new object[] { "icon", "variant" } },
            { "UIToggle", new object[] { "isOn", "labelText" } },
            { "UISlider", new object[] { "value", "minValue", "maxValue", "labelText" } },
            { "UIDropdown", new object[] { "options", "labelText" } },
            { "UIInputField", new object[] { "text", "placeholder", "characterLimit", "contentType" } },
            { "UICheckbox", new object[] { "isOn", "labelText" } },
            { "UIVerticalLayout", new object[] { "spacing", "padding", "childAlignment" } },
            { "UIHorizontalLayout", new object[] { "spacing", "padding", "childAlignment" } },
            { "UIGridLayout", new object[] { "cellSize", "spacing", "constraint" } },
            { "UISpacer", new object[] { "preferredWidth", "preferredHeight" } },
            { "UIPanel", new object[] { "padding" } },
            { "UICard", new object[] { "padding", "elevation" } },
            { "UIScrollView", new object[] { "horizontal", "vertical", "movementType" } },
            { "UIList", new object[] { "spacing", "padding" } },
            { "UIListItem", new object[] { "labelText" } },
            { "UITabContainer", new object[] { "activeTabIndex" } },
            { "UIHeader", new object[] { "title", "showBackButton" } },
            { "UIToolbar", new object[] { "spacing" } },
            { "UIBottomBar", new object[] { "spacing" } },
            { "UITabBar", new object[] { "tabs", "activeIndex" } },
            { "UISidebar", new object[] { "collapsed", "width" } },
            { "UIOverlay", new object[] { "opacity", "blur" } },
            { "UIModal", new object[] { "title", "closeOnBackdrop" } },
            { "UIDialog", new object[] { "title", "message", "actions" } },
            { "UIToast", new object[] { "message", "duration" } },
            { "UITooltip", new object[] { "text", "delay" } },
            { "UIProgressBar", new object[] { "value", "showLabel" } },
            { "UISpinner", new object[] { "color", "speed" } },
            { "UIHealthBar", new object[] { "currentHealth", "maxHealth", "showLabel" } }
        };

        public static void Register(RequestRouter router)
        {
            router.Register("GET", "/api/ui/components/list", HandleListComponents);
        }

        private static async Task HandleListComponents(RequestContext ctx)
        {
            var data = await MainThreadDispatcher.Enqueue(() =>
            {
                var category = ctx.QueryParams.TryGetValue("category", out var c) ? c : "all";
                var map = UIElementHandler.GetComponentToPrefabMap();
                var components = new List<object>();

                foreach (var kv in map)
                {
                    var type = kv.Key;
                    var prefabPath = kv.Value;
                    var categoryPart = prefabPath.Split('/')[0];
                    var cat = CategoryMap.TryGetValue(categoryPart, out var catVal) ? catVal : categoryPart.ToLowerInvariant();

                    if (category != "all" && !string.Equals(cat, category, System.StringComparison.OrdinalIgnoreCase))
                        continue;

                    var prefabVariants = type == "UIButton"
                        ? new[] { "Filled", "Outlined", "Text", "Tonal" }
                        : System.Array.Empty<string>();

                    var properties = ComponentProperties.TryGetValue(type, out var props)
                        ? props
                        : new object[] { "layout", "rect" };

                    components.Add(new
                    {
                        type,
                        category = cat,
                        prefab_path = prefabPath,
                        prefab_variants = prefabVariants,
                        properties
                    });
                }

                return new { components };
            });
            ctx.RespondOk(data);
        }
    }
}
