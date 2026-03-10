using UnityEngine;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum TooltipPosition { Top, Bottom, Left, Right, Auto }

    [RequireComponent(typeof(RectTransform))]
    public class UITooltip : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private RectTransform _arrow;
        [SerializeField] private string text;
        [SerializeField] private TooltipPosition position = TooltipPosition.Top;
        [SerializeField] private float showDelay = 0.5f;

        public override string ComponentType => "UITooltip";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = theme.GetColor("surface");
        }

        public void Show(RectTransform anchor, string t) { text = t; if (_text != null) _text.text = t; gameObject.SetActive(true); }
        public void Hide() => gameObject.SetActive(false);
    }
}
