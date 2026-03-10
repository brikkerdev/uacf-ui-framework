using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class UIListItem : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIIcon _leadingIcon;
        [SerializeField] private UIIcon _trailingIcon;
        [SerializeField] private UIText _title;
        [SerializeField] private UIText _subtitle;
        [SerializeField] private int index;

        public override string ComponentType => "UIListItem";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = style.hasBackground ? style.backgroundColor : theme.GetColor("surface");
            _title?.ReapplyStyle();
            _subtitle?.ReapplyStyle();
        }
    }
}
