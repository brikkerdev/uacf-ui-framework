using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [System.Serializable]
    public class ToolbarAction
    {
        public string iconKey;
        public string label;
        public string id;
        public bool isSelected;
    }

    [RequireComponent(typeof(RectTransform))]
    public class UIToolbar : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIHorizontalLayout _actionsLayout;
        [SerializeField] private string backgroundColorToken = "surface";
        [SerializeField] private List<ToolbarAction> actions = new List<ToolbarAction>();

        public override string ComponentType => "UIToolbar";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = theme.GetColor(backgroundColorToken);
        }

        public void SetActions(List<ToolbarAction> a) { actions = a; }
    }
}
