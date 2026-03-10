using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    public enum UIButtonVariant { Filled, Outlined, Text, Tonal }

    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class UIButton : UIInteractableBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private UnityEngine.UI.Image _icon;
        [SerializeField] private UIButtonVariant variant = UIButtonVariant.Filled;
        [SerializeField] private string labelText = "Button";
        [SerializeField] private string iconKey;
        [SerializeField] private string typographyToken = "button";
        [SerializeField] private UnityEvent onClick;

        public override string ComponentType => "UIButton";

        public UnityEvent OnClick => onClick;

        protected override void Awake()
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
            if (_label != null) _label.text = labelText;
            if (_icon != null) _icon.gameObject.SetActive(!string.IsNullOrEmpty(iconKey));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // Note: Add click listener via UnityEvent in inspector, or add a Button component
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_background == null) _background = GetComponent<UnityEngine.UI.Image>();
            if (_background == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            switch (variant)
            {
                case UIButtonVariant.Filled:
                    _background.color = style.hasBackground ? style.backgroundColor : theme.GetColor("primary");
                    if (_label != null) _label.color = theme.GetColor("onPrimary");
                    break;
                case UIButtonVariant.Outlined:
                    _background.color = new Color(0, 0, 0, 0);
                    if (_label != null) _label.color = theme.GetColor("primary");
                    break;
                case UIButtonVariant.Text:
                    _background.color = new Color(0, 0, 0, 0);
                    if (_label != null) _label.color = theme.GetColor("primary");
                    break;
                case UIButtonVariant.Tonal:
                    var c = theme.GetColor("primaryVariant");
                    c.a = 0.15f;
                    _background.color = style.hasBackground ? style.backgroundColor : c;
                    if (_label != null) _label.color = theme.GetColor("primary");
                    break;
            }

            if (_label != null && style.hasText)
                _label.color = style.textColor;

            theme.GetTypography(typographyToken)?.ApplyTo(_label);
        }

        public void SetLabel(string text) { labelText = text; if (_label != null) _label.text = text; }
        public void SetIcon(string key) { iconKey = key; if (_icon != null) _icon.gameObject.SetActive(!string.IsNullOrEmpty(key)); ReapplyStyle(); }
        public void SetVariant(UIButtonVariant v) { variant = v; ReapplyStyle(); }
        public void SetEnabled(bool enabled) => Interactable = enabled;
    }
}
