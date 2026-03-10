using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIDivider : UIComponentBase
    {
        public enum DividerDirection { Horizontal, Vertical }

        [SerializeField] private UnityEngine.UI.Image _line;
        [SerializeField] private string colorToken = "divider";
        [SerializeField] private float thickness = 1f;
        [SerializeField] private DividerDirection direction = DividerDirection.Horizontal;
        [SerializeField] private string marginToken = "md";

        public override string ComponentType => "UIDivider";

        protected override void Awake()
        {
            if (_line == null) _line = GetComponent<UnityEngine.UI.Image>();
            ApplyThickness();
        }

        private void ApplyThickness()
        {
            var rt = RectTransform;
            if (rt != null)
            {
                if (direction == DividerDirection.Horizontal)
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, thickness);
                else
                    rt.sizeDelta = new Vector2(thickness, rt.sizeDelta.y);
            }
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_line == null) _line = GetComponent<UnityEngine.UI.Image>();
            if (_line == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
                _line.color = theme.GetColor(colorToken);
        }
    }
}
