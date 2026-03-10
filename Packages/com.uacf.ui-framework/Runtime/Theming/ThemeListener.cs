using UnityEngine;
using TMPro;
using UACF.UI.Tokens;

namespace UACF.UI.Theming
{
    [RequireComponent(typeof(RectTransform))]
    public class ThemeListener : MonoBehaviour
    {
        public UnityEngine.UI.Image targetImage;
        public TMP_Text targetText;
        public string imageColorToken = "primary";
        public string textColorToken = "onBackground";

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += OnThemeChanged;
            ApplyTheme(ThemeManager.ActiveTheme);
        }

        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(Theme newTheme)
        {
            ApplyTheme(newTheme);
        }

        private void ApplyTheme(Theme theme)
        {
            if (theme == null) return;

            if (targetImage != null && !string.IsNullOrEmpty(imageColorToken))
                targetImage.color = theme.GetColor(imageColorToken);

            if (targetText != null && !string.IsNullOrEmpty(textColorToken))
                targetText.color = theme.GetColor(textColorToken);
        }
    }
}
