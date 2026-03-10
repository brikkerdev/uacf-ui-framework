using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIIcon : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _image;
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private string iconKey;
        [SerializeField] private string colorToken = "onBackground";
        [SerializeField] private float size = 24f;

        public override string ComponentType => "UIIcon";

        protected override void Awake()
        {
            if (_image == null) _image = GetComponent<UnityEngine.UI.Image>();
            ApplySize();
        }

        private void ApplySize()
        {
            var rt = RectTransform;
            if (rt != null)
            {
                rt.sizeDelta = new Vector2(size, size);
            }
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_image == null) _image = GetComponent<UnityEngine.UI.Image>();
            if (_image == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                if (!string.IsNullOrEmpty(iconKey))
                {
                    var sprite = theme.GetIcon(iconKey);
                    if (sprite != null) _image.sprite = sprite;
                }
                else if (iconSprite != null)
                {
                    _image.sprite = iconSprite;
                }
                _image.color = theme.GetColor(colorToken);
            }
        }

        public void SetIcon(string key) { iconKey = key; ReapplyStyle(); }
        public void SetIcon(Sprite sprite) { iconSprite = sprite; iconKey = null; if (_image != null) _image.sprite = sprite; }
        public void SetColor(string token) { colorToken = token; ReapplyStyle(); }
        public void SetSize(float s) { size = s; ApplySize(); }
    }
}
