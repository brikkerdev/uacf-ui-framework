using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;
using UACF.UI.Screens;

namespace UACF.UI.Components
{
    public enum SidebarSide { Left, Right }

    [RequireComponent(typeof(RectTransform))]
    public class UISidebar : UIComponentBase
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIOverlay _overlay;
        [SerializeField] private float width = 300f;
        [SerializeField] private SidebarSide side = SidebarSide.Left;
        [SerializeField] private bool isOpen;
        [SerializeField] private TransitionPreset openTransition;
        [SerializeField] private string backgroundColorToken = "surface";

        public override string ComponentType => "UISidebar";

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = theme.GetColor(backgroundColorToken);
        }

        public void Open() { isOpen = true; }
        public void Close() { isOpen = false; }
        public void Toggle() { isOpen = !isOpen; }
        public RectTransform GetContent() => _panel;
    }
}
