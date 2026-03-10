using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum ProgressBarVariant { Linear, Indeterminate }

    [RequireComponent(typeof(RectTransform))]
    public class UIProgressBar : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _trackBackground;
        [SerializeField] private UnityEngine.UI.Image _fill;
        [SerializeField] private UIText _label;
        [SerializeField] private float value = 0f;
        [SerializeField] private string fillColorToken = "primary";
        [SerializeField] private string trackColorToken = "disabled";
        [SerializeField] private bool showLabel = false;
        [SerializeField] private string labelFormat = "{0:P0}";
        [SerializeField] private ProgressBarVariant variant = ProgressBarVariant.Linear;

        public override string ComponentType => "UIProgressBar";

        private void Update()
        {
            if (_fill != null) _fill.fillAmount = value;
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                if (_fill != null) _fill.color = theme.GetColor(fillColorToken);
                if (_trackBackground != null) _trackBackground.color = theme.GetColor(trackColorToken);
            }
        }

        public void SetValue(float normalized) { value = Mathf.Clamp01(normalized); }
        public void SetIndeterminate(bool indeterminate) { }
    }
}
