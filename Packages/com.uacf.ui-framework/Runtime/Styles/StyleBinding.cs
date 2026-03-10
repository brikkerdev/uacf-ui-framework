using UnityEngine;
using UACF.UI.Components;
using UACF.UI.Tokens;
using UACF.UI.Theming;

namespace UACF.UI.Styles
{
    [RequireComponent(typeof(UIComponentBase))]
    public class StyleBinding : MonoBehaviour
    {
        public UIStyle style;
        public string styleKey;
        public bool useThemeDefault = true;

        private UIComponentBase _component;
        private Theme _lastTheme;

        private void Awake()
        {
            _component = GetComponent<UIComponentBase>();
        }

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += OnThemeChanged;
            if (_component is UIInteractableBase interactable)
                interactable.OnStateChanged += OnStateChanged;

            ReapplyStyle();
        }

        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= OnThemeChanged;
            if (_component is UIInteractableBase interactable)
                interactable.OnStateChanged -= OnStateChanged;
        }

        private void OnThemeChanged(Theme newTheme)
        {
            _lastTheme = newTheme;
            ReapplyStyle();
        }

        private void OnStateChanged(UIComponentState state)
        {
            ReapplyStyle();
        }

        public void ReapplyStyle()
        {
            if (_component == null) return;

            var theme = ThemeManager.ActiveTheme;
            if (theme == null) return;

            var activeStyle = style;
            if (activeStyle == null && useThemeDefault)
                activeStyle = theme.GetDefaultStyle(_component.ComponentType);

            if (activeStyle == null) return;

            var state = _component is UIInteractableBase interactable
                ? interactable.CurrentState
                : UIComponentState.Normal;

            var resolved = StyleResolver.Resolve(activeStyle, state, theme);
            _component.ApplyStyle(resolved);
        }
    }
}
