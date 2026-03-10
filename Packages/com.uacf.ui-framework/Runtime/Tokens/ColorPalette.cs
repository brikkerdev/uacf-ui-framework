using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Tokens/Color Palette", fileName = "ColorPalette")]
    public class ColorPalette : ScriptableObject
    {
        [Header("Brand")]
        public Color primary = new Color(0.384f, 0f, 0.933f, 1f);
        public Color primaryVariant = new Color(0.2f, 0f, 0.6f, 1f);
        public Color secondary = new Color(0.012f, 0.588f, 0.533f, 1f);
        public Color secondaryVariant = new Color(0f, 0.4f, 0.4f, 1f);
        public Color accent = new Color(1f, 0.6f, 0f, 1f);

        [Header("Surfaces")]
        public Color background = Color.white;
        public Color surface = new Color(0.98f, 0.98f, 0.98f, 1f);
        public Color surfaceVariant = new Color(0.95f, 0.95f, 0.95f, 1f);

        [Header("Semantic")]
        public Color error = new Color(0.9f, 0.2f, 0.2f, 1f);
        public Color warning = new Color(1f, 0.6f, 0f, 1f);
        public Color success = new Color(0.2f, 0.7f, 0.3f, 1f);
        public Color info = new Color(0.2f, 0.5f, 0.9f, 1f);

        [Header("On-Colors")]
        public Color onPrimary = Color.white;
        public Color onSecondary = Color.white;
        public Color onBackground = new Color(0.1f, 0.1f, 0.1f, 1f);
        public Color onBackgroundSecondary = new Color(0.45f, 0.45f, 0.45f, 1f);
        public Color onSurface = new Color(0.1f, 0.1f, 0.1f, 1f);
        public Color onError = Color.white;

        [Header("Utility")]
        public Color divider = new Color(0.9f, 0.9f, 0.9f, 1f);
        public Color disabled = new Color(0.7f, 0.7f, 0.7f, 1f);
        public Color overlay = new Color(0f, 0f, 0f, 0.5f);
        public Color shadow = new Color(0f, 0f, 0f, 0.25f);

        [Header("Custom")]
        public List<NamedColor> customColors = new List<NamedColor>();

        private static readonly Dictionary<string, Func<ColorPalette, Color>> SemanticGetters;

        static ColorPalette()
        {
            SemanticGetters = new Dictionary<string, Func<ColorPalette, Color>>(StringComparer.OrdinalIgnoreCase)
            {
                { "primary", p => p.primary },
                { "primaryVariant", p => p.primaryVariant },
                { "secondary", p => p.secondary },
                { "secondaryVariant", p => p.secondaryVariant },
                { "accent", p => p.accent },
                { "background", p => p.background },
                { "surface", p => p.surface },
                { "surfaceVariant", p => p.surfaceVariant },
                { "error", p => p.error },
                { "warning", p => p.warning },
                { "success", p => p.success },
                { "info", p => p.info },
                { "onPrimary", p => p.onPrimary },
                { "onSecondary", p => p.onSecondary },
                { "onBackground", p => p.onBackground },
                { "onBackgroundSecondary", p => p.onBackgroundSecondary },
                { "onSurface", p => p.onSurface },
                { "onError", p => p.onError },
                { "divider", p => p.divider },
                { "disabled", p => p.disabled },
                { "overlay", p => p.overlay },
                { "shadow", p => p.shadow }
            };
        }

        public Color GetColor(string key)
        {
            if (string.IsNullOrEmpty(key)) return Color.magenta;

            if (SemanticGetters.TryGetValue(key, out var getter))
                return getter(this);

            var idx = customColors.FindIndex(c => string.Equals(c.key, key, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                return customColors[idx].color;

            Debug.LogWarning($"[ColorPalette] Color key '{key}' not found. Returning magenta.");
            return Color.magenta;
        }

        public bool TryGetColor(string key, out Color color)
        {
            if (string.IsNullOrEmpty(key))
            {
                color = Color.magenta;
                return false;
            }

            if (SemanticGetters.TryGetValue(key, out var getter))
            {
                color = getter(this);
                return true;
            }

            var idx = customColors.FindIndex(c => string.Equals(c.key, key, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
            {
                color = customColors[idx].color;
                return true;
            }

            color = Color.magenta;
            return false;
        }

        public void SetColor(string key, Color color)
        {
            if (SemanticGetters.ContainsKey(key))
            {
                Debug.LogWarning($"[ColorPalette] Cannot set semantic color '{key}' via SetColor.");
                return;
            }

            var idx = customColors.FindIndex(c => string.Equals(c.key, key, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                customColors[idx] = new NamedColor(key, color, customColors[idx].description);
            else
                customColors.Add(new NamedColor(key, color));
        }

        public List<string> GetAllColorKeys()
        {
            var list = new List<string>(SemanticGetters.Keys);
            foreach (var c in customColors)
            {
                if (!string.IsNullOrEmpty(c.key) && !list.Contains(c.key))
                    list.Add(c.key);
            }
            return list;
        }

        public Color GetColorWithAlpha(string key, float alpha)
        {
            var c = GetColor(key);
            c.a = Mathf.Clamp01(alpha);
            return c;
        }
    }
}
