using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIPanel : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private string backgroundColorToken = "surface";
        [SerializeField] private string shapeToken = "medium";
        [SerializeField] private string elevationToken = "none";
        [SerializeField] private string paddingToken = "md";
        [SerializeField] private RectOffset paddingOverride;

        public override string ComponentType => "UIPanel";

        protected override void Awake()
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
            if (_background == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                _background.color = style.hasBackground ? style.backgroundColor : theme.GetColor(backgroundColorToken);
            }
        }
    }
}
