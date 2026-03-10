using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIImage : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _image;
        [SerializeField] private Sprite sprite;
        [SerializeField] private string colorToken = "primary";
        [SerializeField] private UnityEngine.UI.Image.Type imageType = UnityEngine.UI.Image.Type.Simple;
        [SerializeField] private bool preserveAspect = true;

        public override string ComponentType => "UIImage";

        protected override void Awake()
        {
            if (_image == null) _image = GetComponent<UnityEngine.UI.Image>();
            if (sprite != null && _image != null) _image.sprite = sprite;
            if (_image != null) _image.preserveAspect = preserveAspect;
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_image == null) _image = GetComponent<UnityEngine.UI.Image>();
            if (_image == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
                _image.color = style.hasBackground ? style.backgroundColor : theme.GetColor(colorToken);

            if (style.hasBackground)
                _image.color = style.backgroundColor;
        }

        public void SetSprite(Sprite s) { sprite = s; if (_image != null) _image.sprite = s; }
        public void SetColor(string token) { colorToken = token; ReapplyStyle(); }
        public void SetNativeSize() { if (_image != null) _image.SetNativeSize(); }
    }
}
