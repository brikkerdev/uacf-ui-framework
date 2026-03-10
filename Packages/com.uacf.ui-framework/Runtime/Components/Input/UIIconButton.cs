using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum UIIconButtonShape { Circle, Square, Rounded }

    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIIconButton : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UnityEngine.UI.Image _icon;
        [SerializeField] private string iconKey;
        [SerializeField] private string colorToken = "primary";
        [SerializeField] private float size = 48f;
        [SerializeField] private UIIconButtonShape shape = UIIconButtonShape.Circle;

        public override string ComponentType => "UIIconButton";

        protected override void Awake()
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
            RectTransform.sizeDelta = new Vector2(size, size);
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
            if (_background == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                _background.color = style.hasBackground ? style.backgroundColor : theme.GetColor(colorToken);
                if (_icon != null && !string.IsNullOrEmpty(iconKey))
                {
                    var sprite = theme.GetIcon(iconKey);
                    if (sprite != null) _icon.sprite = sprite;
                    _icon.color = theme.GetColor(colorToken);
                }
            }
        }

        public void SetIcon(string key)
        {
            iconKey = key;
            ReapplyStyle();
        }
    }
}
