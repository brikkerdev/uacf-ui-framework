using UnityEngine;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UISpinner : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _spinner;
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private float size = 48f;
        [SerializeField] private string colorToken = "primary";

        public override string ComponentType => "UISpinner";

        private void Update()
        {
            if (_spinner != null)
                _spinner.transform.Rotate(0, 0, -rotationSpeed * Time.unscaledDeltaTime);
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null && _spinner != null)
                _spinner.color = theme.GetColor(colorToken);
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
    }
}
