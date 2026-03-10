using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class UICheckbox : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _box;
        [SerializeField] private UnityEngine.UI.Image _checkmark;
        [SerializeField] private UIText _label;
        [SerializeField] private UnityEngine.UI.Toggle _toggle;
        [SerializeField] private string checkedColorToken = "primary";
        [SerializeField] private string uncheckedColorToken = "onBackground";

        public UnityEvent<bool> onValueChanged;

        public bool IsChecked => _toggle != null && _toggle.isOn;
        public override string ComponentType => "UICheckbox";

        protected override void Awake()
        {
            if (_toggle == null) _toggle = GetComponent<UnityEngine.UI.Toggle>();
            if (_toggle != null)
                _toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool value)
        {
            onValueChanged?.Invoke(value);
            ReapplyStyle();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (_checkmark != null)
                _checkmark.gameObject.SetActive(IsChecked);
            if (_box != null)
                _box.color = IsChecked ? theme.GetColor(checkedColorToken) : theme.GetColor(uncheckedColorToken);
        }

        public void SetChecked(bool value) { if (_toggle != null) _toggle.isOn = value; }
    }
}
