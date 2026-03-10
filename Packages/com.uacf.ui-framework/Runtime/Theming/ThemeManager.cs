using System;
using System.Collections.Generic;
using UnityEngine;
using UACF.UI.Tokens;

namespace UACF.UI.Theming
{
    public class ThemeManager : MonoBehaviour
    {
        private static ThemeManager _instance;
        private static Theme _activeTheme;
        private static Theme _defaultTheme;
        private static readonly List<IThemeable> Themeables = new List<IThemeable>();

        private const string DefaultThemePath = "UACF_UI/DefaultTheme/DefaultTheme";

        public static event Action<Theme> OnThemeChanged;

        public static Theme ActiveTheme
        {
            get
            {
                EnsureInitialized();
                return _activeTheme ?? _defaultTheme;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInitialized()
        {
            if (_instance != null) return;

            var go = new GameObject("[ThemeManager]");
            _instance = go.AddComponent<ThemeManager>();
            DontDestroyOnLoad(go);

            _defaultTheme = Resources.Load<Theme>(DefaultThemePath);
            _activeTheme = _defaultTheme;

            if (_defaultTheme == null)
                Debug.LogWarning("[ThemeManager] Default theme not found at Resources/" + DefaultThemePath);
        }

        public static void SetTheme(Theme theme)
        {
            EnsureInitialized();
            if (theme == _activeTheme) return;

            _activeTheme = theme;
            OnThemeChanged?.Invoke(theme);

            for (var i = Themeables.Count - 1; i >= 0; i--)
            {
                if (Themeables[i] != null)
                    Themeables[i].OnThemeChanged(theme);
            }
        }

        public static void SetTheme(string themeId)
        {
            var theme = GetThemeById(themeId);
            if (theme != null)
                SetTheme(theme);
            else
                Debug.LogWarning($"[ThemeManager] Theme with id '{themeId}' not found.");
        }

        public static void RegisterThemeable(IThemeable themeable)
        {
            if (themeable != null && !Themeables.Contains(themeable))
                Themeables.Add(themeable);
        }

        public static void UnregisterThemeable(IThemeable themeable)
        {
            Themeables.Remove(themeable);
        }

        public static Color GetColor(string key)
        {
            return ActiveTheme?.GetColor(key) ?? Color.magenta;
        }

        public static Tokens.TypographyPreset GetTypography(string key)
        {
            return ActiveTheme?.GetTypography(key);
        }

        public static float GetSpacing(string key)
        {
            return ActiveTheme?.GetSpacing(key) ?? 0f;
        }

        public static List<Theme> GetAvailableThemes()
        {
            var themes = new List<Theme>();
            var assets = Resources.LoadAll<Theme>("");
            themes.AddRange(assets);
            return themes;
        }

        private static Theme GetThemeById(string themeId)
        {
            var themes = GetAvailableThemes();
            foreach (var t in themes)
            {
                if (string.Equals(t.themeId, themeId, StringComparison.OrdinalIgnoreCase))
                    return t;
            }
            return null;
        }
    }
}
