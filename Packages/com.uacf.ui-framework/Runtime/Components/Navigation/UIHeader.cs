using UnityEngine;
using System;
using UACF.UI.Tokens;
using UACF.UI.Theming;
using UACF.UI.Styles;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class UIHeader : UIComponentBase
    {
        [SerializeField] private UnityEngine.UI.Image _background;
        [SerializeField] private UIText _title;
        [SerializeField] private UIIconButton _leftAction;
        [SerializeField] private UIIconButton _rightAction;
        [SerializeField] private string backgroundColorToken = "surface";
        [SerializeField] private string titleTypography = "h3";
        [SerializeField] private string elevationToken = "low";
        [SerializeField] private float height = 56f;

        public override string ComponentType => "UIHeader";

        protected override void Awake()
        {
            var rt = RectTransform;
            if (rt != null) rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }

        public override void ApplyStyle(ResolvedStyle style)
        {
            var theme = ThemeManager.ActiveTheme;
            if (theme != null)
            {
                if (_background != null) _background.color = theme.GetColor(backgroundColorToken);
                theme.GetTypography(titleTypography)?.ApplyTo(_title?.GetComponent<TMPro.TMP_Text>());
            }
        }

        public void SetTitle(string title) { if (_title != null) _title.SetText(title); }
        public void SetLeftAction(string iconKey, Action callback) { if (_leftAction != null) { _leftAction.SetIcon(iconKey); } }
        public void SetRightAction(string iconKey, Action callback) { if (_rightAction != null) { _rightAction.SetIcon(iconKey); } }
        public void ShowBackButton(Action onBack) { SetLeftAction("back", onBack); }
    }
}
