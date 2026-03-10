using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollView : UIComponentBase
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private Scrollbar _verticalScrollbar;
        [SerializeField] private Scrollbar _horizontalScrollbar;
        [SerializeField] private bool showVerticalScrollbar = true;
        [SerializeField] private bool showHorizontalScrollbar = false;
        [SerializeField] private string scrollbarColorToken = "disabled";

        public override string ComponentType => "UIScrollView";

        protected override void Awake()
        {
            if (_scrollRect == null) _scrollRect = GetComponent<ScrollRect>();
            if (_content == null && _scrollRect != null) _content = _scrollRect.content;
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                if (_verticalScrollbar != null) _verticalScrollbar.gameObject.SetActive(showVerticalScrollbar);
                if (_horizontalScrollbar != null) _horizontalScrollbar.gameObject.SetActive(showHorizontalScrollbar);
            }
        }

        public RectTransform GetContent() => _content;
        public void ScrollTo(float normalizedPosition) { if (_scrollRect != null) _scrollRect.verticalNormalizedPosition = normalizedPosition; }
        public void ScrollToTop() => ScrollTo(1f);
        public void ScrollToBottom() => ScrollTo(0f);
    }
}
