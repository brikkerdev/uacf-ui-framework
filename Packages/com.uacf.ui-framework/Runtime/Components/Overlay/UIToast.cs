using UnityEngine;
using System;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum ToastPosition { Top, Bottom, Center }

    [RequireComponent(typeof(RectTransform))]
    public class UIToast : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIText _message;
        [SerializeField] private UIIconButton _action;
        [SerializeField] private float duration = 3f;
        [SerializeField] private ToastPosition position = ToastPosition.Bottom;

        public override string ComponentType => "UIToast";

        public override void ApplyStyle(ResolvedStyle style)
        {
            _message?.ReapplyStyle();
        }

        public static UIToast Show(string message, float duration = 3f) => null;
        public static UIToast Show(string message, string actionLabel, Action onAction) => null;
    }
}
