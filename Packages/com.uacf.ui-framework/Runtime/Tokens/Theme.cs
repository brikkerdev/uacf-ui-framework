using System;
using UnityEngine;
using UACF.UI.Styles;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Theme", fileName = "Theme")]
    public class Theme : ScriptableObject
    {
        public string themeName = "Default";
        public string themeId;

        [Header("Token Sets")]
        public ColorPalette colorPalette;
        public TypographySet typography;
        public SpacingScale spacing;
        public ShapeSet shapes;
        public ElevationSet elevations;
        public IconSet icons;

        [Header("Default Styles")]
        public StyleSheet defaultStyles;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(themeId) && !string.IsNullOrEmpty(themeName))
                themeId = themeName.ToLowerInvariant().Replace(" ", "-");
        }

        public Color GetColor(string key)
        {
            return colorPalette != null ? colorPalette.GetColor(key) : Color.magenta;
        }

        public TypographyPreset GetTypography(string key)
        {
            return typography != null ? typography.GetPreset(key) : null;
        }

        public float GetSpacing(string key)
        {
            return spacing != null ? spacing.GetSpacing(key) : 0f;
        }

        public ShapePreset GetShape(string key)
        {
            return shapes != null ? shapes.GetPreset(key) : null;
        }

        public ElevationPreset GetElevation(string key)
        {
            return elevations != null ? elevations.GetPreset(key) : null;
        }

        public Sprite GetIcon(string key)
        {
            return icons != null ? icons.GetIcon(key) : null;
        }

        public UIStyle GetDefaultStyle(string componentType)
        {
            return defaultStyles != null ? defaultStyles.GetStyle(componentType) : null;
        }
    }
}
