using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class UIToggle : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _track;
        [SerializeField] private UnityEngine.UI.Image _thumb;
        [SerializeField] private UnityEngine.UI.Toggle _toggle;
        [SerializeField] private string onColorToken = "primary";
        [SerializeField] private string offColorToken = "disabled";
        [SerializeField] private string thumbColorToken = "surface";

        public UnityEvent<bool> onValueChanged;

        public bool IsOn => _toggle != null && _toggle.isOn;
        public override string ComponentType => "UIToggle";

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

            if (_track != null)
                _track.color = IsOn ? theme.GetColor(onColorToken) : theme.GetColor(offColorToken);
            if (_thumb != null)
                _thumb.color = theme.GetColor(thumbColorToken);
        }

        public void SetOn(bool value) { if (_toggle != null) _toggle.isOn = value; }
    }
}
