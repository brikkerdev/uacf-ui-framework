using UnityEngine;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIText : UIComponentBase
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private string text = "";
        [SerializeField] private string typographyToken = "body1";
        [SerializeField] private string colorToken = "onBackground";
        [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;
        [SerializeField] private bool useAlignmentOverride;
        [SerializeField] private bool autoSize = false;

        public override string ComponentType => "UIText";

        public string Text
        {
            get => text;
            set { text = value; if (_tmpText != null) _tmpText.text = value; }
        }

        protected override void Awake()
        {
            if (_tmpText == null) _tmpText = GetComponent<TextMeshProUGUI>();
            if (!string.IsNullOrEmpty(text) && _tmpText != null) _tmpText.text = text;
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            if (_tmpText == null) _tmpText = GetComponent<TextMeshProUGUI>();
            if (_tmpText == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (style.hasText)
            {
                _tmpText.color = style.textColor;
            }
            else
            {
                _tmpText.color = theme.GetColor(colorToken);
            }

            if (style.typography != null)
            {
                style.typography.ApplyTo(_tmpText);
            }
            else
            {
                theme.GetTypography(typographyToken)?.ApplyTo(_tmpText);
            }

            if (useAlignmentOverride)
                _tmpText.alignment = alignment;

            _tmpText.enableAutoSizing = autoSize;
        }

        public void SetText(string value) => Text = value;
        public void SetTypography(string token) { typographyToken = token; ReapplyStyle(); }
        public void SetColor(string token) { colorToken = token; ReapplyStyle(); }
    }
}
