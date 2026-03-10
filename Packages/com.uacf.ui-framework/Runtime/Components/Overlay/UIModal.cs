using UnityEngine;
using UnityEngine.Events;
using System;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;
using UACF.UI.Screens;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIModal : UIComponentBase
    {
        [SerializeField] private UIOverlay _overlay;
        [SerializeField] private UIPanel _panel;
        [SerializeField] private RectTransform _contentSlot;
        [SerializeField] private UIHeader _header;
        [SerializeField] private RectTransform _actionsSlot;
        [SerializeField] private TransitionPreset showTransition;
        [SerializeField] private bool closeOnOverlayTap = true;
        [SerializeField] private float maxWidth = 400f;
        [SerializeField] private float maxHeight = 600f;
        [SerializeField] private string overlayColorToken = "overlay";

        public UnityEvent onClose;

        public override string ComponentType => "UIModal";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _overlay?.ReapplyStyle();
            _panel?.ReapplyStyle();
            _header?.ReapplyStyle();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() { gameObject.SetActive(false); onClose?.Invoke(); }
        public void SetTitle(string title) { if (_header != null) _header.SetTitle(title); }
        public RectTransform GetContentSlot() => _contentSlot;
        public RectTransform GetActionsSlot() => _actionsSlot;
        public void AddAction(string label, UIButtonVariant variant, Action callback) { }
    }
}
