using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum UIInputFieldVariant { Outlined, Filled, Underlined }

    [RequireComponent(typeof(TMP_InputField))]
    public class UIInputField : UIInteractableBase
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMP_Text _placeholder;
        [SerializeField] private TMP_Text _inputText;
        [SerializeField] private UIText _label;
        [SerializeField] private UIText _helperText;
        [SerializeField] private UIText _errorText;
        [SerializeField] private string labelText;
        [SerializeField] private string placeholderText;
        [SerializeField] private string helperText;
        [SerializeField] private string errorMessage;
        [SerializeField] private bool hasError;
        [SerializeField] private UIInputFieldVariant variant = UIInputFieldVariant.Outlined;
        [SerializeField] private string focusColorToken = "primary";
        [SerializeField] private string errorColorToken = "error";

        public UnityEvent<string> onValueChanged;
        public UnityEvent<string> onEndEdit;

        public string Text => _inputField != null ? _inputField.text : "";
        public override string ComponentType => "UIInputField";

        protected override void Awake()
        {
            if (_inputField == null) _inputField = GetComponent<TMP_InputField>();
            if (_inputField != null)
            {
                _inputField.onValueChanged.AddListener(s => onValueChanged?.Invoke(s));
                _inputField.onEndEdit.AddListener(s => onEndEdit?.Invoke(s));
            }
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (_background != null)
                _background.color = hasError ? theme.GetColor(errorColorToken) : theme.GetColor("surface");
        }

        public void SetText(string text) { if (_inputField != null) _inputField.text = text; }
        public void SetError(string msg) { errorMessage = msg; hasError = !string.IsNullOrEmpty(msg); if (_errorText != null) _errorText.SetText(msg); ReapplyStyle(); }
        public void ClearError() { errorMessage = ""; hasError = false; if (_errorText != null) _errorText.SetText(""); ReapplyStyle(); }
        public void SetLabel(string text) { labelText = text; if (_label != null) _label.SetText(text); }
        public void SetPlaceholder(string text) { placeholderText = text; if (_placeholder != null) _placeholder.text = text; }
    }
}
