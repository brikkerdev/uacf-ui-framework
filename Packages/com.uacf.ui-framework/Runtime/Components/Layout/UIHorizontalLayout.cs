using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class UIHorizontalLayout : UIComponentBase
    {
        [SerializeField] private HorizontalLayoutGroup _layoutGroup;
        [SerializeField] private ContentSizeFitter _sizeFitter;
        [SerializeField] private string spacingToken = "md";
        [SerializeField] private string paddingToken = "none";
        [SerializeField] private RectOffset paddingOverride;
        [SerializeField] private float spacingOverride = -1f;
        [SerializeField] private TextAnchor childAlignment = TextAnchor.MiddleLeft;
        [SerializeField] private bool childForceExpandWidth = false;
        [SerializeField] private bool childForceExpandHeight = true;

        public override string ComponentType => "UIHorizontalLayout";

        protected override void Awake()
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_layoutGroup == null) _layoutGroup = GetComponent<HorizontalLayoutGroup>();
            if (_layoutGroup == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                _layoutGroup.spacing = spacingOverride >= 0 ? spacingOverride : theme.GetSpacing(spacingToken);
                _layoutGroup.padding = paddingOverride != null ? paddingOverride : theme.spacing.GetPadding(paddingToken);
            }
            _layoutGroup.childAlignment = childAlignment;
            _layoutGroup.childForceExpandWidth = childForceExpandWidth;
            _layoutGroup.childForceExpandHeight = childForceExpandHeight;
        }

        public void SetSpacing(string token) { spacingToken = token; ReapplyStyle(); }
        public void SetSpacing(float value) { spacingOverride = value; ReapplyStyle(); }
        public void SetPadding(string token) { paddingToken = token; paddingOverride = null; ReapplyStyle(); }
        public void SetPadding(RectOffset padding) { paddingOverride = padding; ReapplyStyle(); }
    }
}
