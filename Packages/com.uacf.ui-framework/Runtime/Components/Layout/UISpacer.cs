using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum SpacerDirection { Auto, Horizontal, Vertical, Expand }

    [RequireComponent(typeof(LayoutElement))]
    public class UISpacer : UIComponentBase
    {
        [SerializeField] private string sizeToken = "md";
        [SerializeField] private float sizeOverride = -1f;
        [SerializeField] private SpacerDirection direction = SpacerDirection.Auto;

        private LayoutElement _layoutElement;

        public override string ComponentType => "UISpacer";

        protected override void Awake()
        {
            _layoutElement = GetComponent<LayoutElement>();
            if (_layoutElement == null) _layoutElement = gameObject.AddComponent<LayoutElement>();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_layoutElement == null) _layoutElement = GetComponent<LayoutElement>();
            if (_layoutElement == null) return;

            var theme = ThemeManager.ActiveTheme;
            var size = sizeOverride >= 0 ? sizeOverride : (theme != null ? theme.GetSpacing(sizeToken) : 0f);

            if (direction == SpacerDirection.Expand)
            {
                _layoutElement.flexibleWidth = 1;
                _layoutElement.flexibleHeight = 1;
                _layoutElement.preferredWidth = -1;
                _layoutElement.preferredHeight = -1;
            }
            else if (direction == SpacerDirection.Horizontal)
            {
                _layoutElement.preferredWidth = size;
                _layoutElement.preferredHeight = -1;
                _layoutElement.flexibleWidth = 0;
                _layoutElement.flexibleHeight = 0;
            }
            else if (direction == SpacerDirection.Vertical)
            {
                _layoutElement.preferredWidth = -1;
                _layoutElement.preferredHeight = size;
                _layoutElement.flexibleWidth = 0;
                _layoutElement.flexibleHeight = 0;
            }
            else
            {
                _layoutElement.preferredWidth = size;
                _layoutElement.preferredHeight = size;
                _layoutElement.flexibleWidth = 0;
                _layoutElement.flexibleHeight = 0;
            }
        }
    }
}
