using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UACF.UI.Editor.UACF
{
    /// <summary>
    /// Default LayoutElement values per component type for Layout First approach.
    /// -1 = not set (layout group decides).
    /// </summary>
    public static class LayoutElementDefaults
    {
        public struct LayoutDef
        {
            public float preferredWidth;
            public float preferredHeight;
            public float flexibleWidth;
            public float flexibleHeight;
            public float minWidth;
            public float minHeight;

            public static LayoutDef Create(float pw, float ph, float fw, float fh, float mw = -1, float mh = -1)
            {
                return new LayoutDef
                {
                    preferredWidth = pw,
                    preferredHeight = ph,
                    flexibleWidth = fw,
                    flexibleHeight = fh,
                    minWidth = mw,
                    minHeight = mh
                };
            }
        }

        private static readonly Dictionary<string, LayoutDef> Defaults = new()
        {
            // Display
            { "UIText", LayoutDef.Create(-1, -1, 1, 0) },
            { "UIImage", LayoutDef.Create(200, 40, 0, 0) },
            { "UIIcon", LayoutDef.Create(24, 24, 0, 0) },
            { "UIDivider", LayoutDef.Create(-1, 1, 1, 0) },
            { "UIBadge", LayoutDef.Create(-1, 24, 0, 0) },
            // Input
            { "UIButton", LayoutDef.Create(120, 40, 0, 0) },
            { "UIIconButton", LayoutDef.Create(40, 40, 0, 0) },
            { "UIToggle", LayoutDef.Create(48, 24, 0, 0) },
            { "UISlider", LayoutDef.Create(-1, 24, 1, 0) },
            { "UIDropdown", LayoutDef.Create(-1, 40, 1, 0) },
            { "UIInputField", LayoutDef.Create(-1, 40, 1, 0) },
            { "UICheckbox", LayoutDef.Create(24, 24, 0, 0) },
            // Layout (UISpacer has its own LayoutElement, others are containers)
            { "UIPanel", LayoutDef.Create(-1, -1, 1, 1) },
            { "UICard", LayoutDef.Create(-1, -1, 1, 0) },
            { "UIScrollView", LayoutDef.Create(-1, -1, 1, 1) },
            { "UIList", LayoutDef.Create(-1, -1, 1, 1) },
            { "UIListItem", LayoutDef.Create(-1, 48, 1, 0) },
            { "UITabContainer", LayoutDef.Create(-1, -1, 1, 1) },
            // Navigation
            { "UIHeader", LayoutDef.Create(-1, 56, 1, 0) },
            { "UIToolbar", LayoutDef.Create(-1, 48, 1, 0) },
            { "UIBottomBar", LayoutDef.Create(-1, 56, 1, 0) },
            { "UITabBar", LayoutDef.Create(-1, 48, 1, 0) },
            { "UISidebar", LayoutDef.Create(200, -1, 0, 1) },
            // Overlay
            { "UIOverlay", LayoutDef.Create(-1, -1, 1, 1) },
            { "UIModal", LayoutDef.Create(400, -1, 0, 0) },
            { "UIDialog", LayoutDef.Create(400, -1, 0, 0) },
            { "UIToast", LayoutDef.Create(-1, 48, 1, 0) },
            { "UITooltip", LayoutDef.Create(-1, -1, 0, 0) },
            // Feedback
            { "UIProgressBar", LayoutDef.Create(-1, 8, 1, 0) },
            { "UISpinner", LayoutDef.Create(24, 24, 0, 0) },
            { "UIHealthBar", LayoutDef.Create(-1, 24, 1, 0) }
        };

        public static bool TryGet(string componentType, out LayoutDef def)
        {
            return Defaults.TryGetValue(componentType, out def);
        }

        public static void ApplyTo(LayoutElement le, string componentType)
        {
            if (!TryGet(componentType, out var def))
                return;

            if (def.preferredWidth >= 0) le.preferredWidth = def.preferredWidth;
            if (def.preferredHeight >= 0) le.preferredHeight = def.preferredHeight;
            if (def.flexibleWidth >= 0) le.flexibleWidth = def.flexibleWidth;
            if (def.flexibleHeight >= 0) le.flexibleHeight = def.flexibleHeight;
            if (def.minWidth >= 0) le.minWidth = def.minWidth;
            if (def.minHeight >= 0) le.minHeight = def.minHeight;
        }

        public static LayoutElement GetOrAdd(GameObject go)
        {
            var le = go.GetComponent<LayoutElement>();
            if (le == null)
                le = go.AddComponent<LayoutElement>();
            return le;
        }
    }
}
