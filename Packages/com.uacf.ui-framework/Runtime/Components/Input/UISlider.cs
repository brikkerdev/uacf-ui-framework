using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(Slider))]
    public class UISlider : UIInteractableBase
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private UnityEngine.UI.Image _trackBackground;
        [SerializeField] private UnityEngine.UI.Image _trackFill;
        [SerializeField] private UnityEngine.UI.Image _thumb;
        [SerializeField] private string fillColorToken = "primary";
        [SerializeField] private string trackColorToken = "disabled";
        [SerializeField] private UIText _valueLabel;
        [SerializeField] private string valueFormat = "F1";

        public UnityEvent<float> onValueChanged;

        public float Value => _slider != null ? _slider.value : 0f;
        public override string ComponentType => "UISlider";

        protected override void Awake()
        {
            if (_slider == null) _slider = GetComponent<Slider>();
            if (_slider != null)
                _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            onValueChanged?.Invoke(value);
            if (_valueLabel != null)
                _valueLabel.SetText(value.ToString(valueFormat));
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (_trackFill != null) _trackFill.color = theme.GetColor(fillColorToken);
            if (_trackBackground != null) _trackBackground.color = theme.GetColor(trackColorToken);
        }

        public void SetValue(float value) { if (_slider != null) _slider.value = value; }
        public void SetRange(float min, float max) { if (_slider != null) { _slider.minValue = min; _slider.maxValue = max; } }
    }
}
