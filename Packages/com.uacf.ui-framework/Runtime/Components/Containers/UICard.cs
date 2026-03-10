using UnityEngine;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UICard : UIComponentBase
    {
        [SerializeField] private UIPanel _panel;
        [SerializeField] private RectTransform _headerSlot;
        [SerializeField] private RectTransform _contentSlot;
        [SerializeField] private RectTransform _actionsSlot;
        [SerializeField] private UIText _title;
        [SerializeField] private UIText _subtitle;
        [SerializeField] private string elevationToken = "low";
        [SerializeField] private bool showDividers = true;

        public override string ComponentType => "UICard";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _panel?.ReapplyStyle();
            _title?.ReapplyStyle();
            _subtitle?.ReapplyStyle();
        }

        public void SetTitle(string title) { if (_title != null) _title.SetText(title); }
        public void SetSubtitle(string subtitle) { if (_subtitle != null) _subtitle.SetText(subtitle); }
        public RectTransform GetContentSlot() => _contentSlot;
        public RectTransform GetActionsSlot() => _actionsSlot;
    }
}
