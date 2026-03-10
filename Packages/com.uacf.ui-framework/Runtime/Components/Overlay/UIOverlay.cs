using UnityEngine;
using UnityEngine.Events;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIOverlay : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private string colorToken = "overlay";
        [SerializeField] private bool blockRaycasts = true;

        public UnityEvent onTap;

        public override string ComponentType => "UIOverlay";

        protected override void Awake()
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _background != null)
                _background.color = theme.GetColor(colorToken);
        }

        public void Show(float fadeInDuration = 0.2f) => gameObject.SetActive(true);
        public void Hide(float fadeOutDuration = 0.2f) => gameObject.SetActive(false);
    }
}
