using UnityEngine;
using TMPro;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIBadge : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string backgroundColorToken = "error";
        [SerializeField] private string textColorToken = "onError";
        [SerializeField] private int value;
        [SerializeField] private int maxDisplay = 99;
        [SerializeField] private bool showZero = false;

        public override string ComponentType => "UIBadge";

        protected override void Awake()
        {
            RefreshDisplay();
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            if (_background != null)
                _background.color = theme.GetColor(backgroundColorToken);
            if (_label != null)
                _label.color = theme.GetColor(textColorToken);
        }

        public void SetValue(int v)
        {
            value = v;
            RefreshDisplay();
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void RefreshDisplay()
        {
            var show = value > 0 || showZero;
            gameObject.SetActive(show);
            if (_label != null)
                _label.text = value > maxDisplay ? $"{maxDisplay}+" : value.ToString();
        }
    }
}
