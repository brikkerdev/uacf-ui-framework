using UnityEngine;
using System;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDialog : UIComponentBase
    {
        [SerializeField] private UIModal _modal;
        [SerializeField] private UIText _message;
        [SerializeField] private UIButton _positiveButton;
        [SerializeField] private UIButton _negativeButton;
        [SerializeField] private UIButton _neutralButton;

        public override string ComponentType => "UIDialog";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _modal?.ReapplyStyle();
        }

        public static UIDialog Show(string title, string message, string positiveLabel = "OK", string negativeLabel = null, Action onPositive = null, Action onNegative = null)
        {
            return null;
        }

        public static UIDialog ShowConfirm(string title, string message, Action onConfirm, Action onCancel = null)
        {
            return null;
        }
    }
}
