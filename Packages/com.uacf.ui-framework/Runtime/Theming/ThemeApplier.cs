using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;

namespace UACF.UI.Theming
{
    public class ThemeApplier : MonoBehaviour
    {
        public Theme overrideTheme;
        public bool applyToChildren = true;

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
            ApplyTheme(overrideTheme ?? newTheme);
        }

        private void ApplyTheme(Theme theme)
        {
            var effectiveTheme = overrideTheme ?? theme;
            if (effectiveTheme == null) return;

            var themeables = new List<IThemeable>();
            if (applyToChildren)
                GetComponentsInChildren(true, themeables);
            else
                GetComponents(themeables);

            foreach (var t in themeables)
            {
                if (t != null)
                    t.OnThemeChanged(effectiveTheme);
            }
        }
    }
}
