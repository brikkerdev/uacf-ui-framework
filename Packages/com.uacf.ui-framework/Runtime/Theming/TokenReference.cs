using System;
using UnityEngine;
using TMPro;
using UACF.UI.Tokens;

namespace UACF.UI.Theming
{
    [Serializable]
    public class TokenReference<T>
    {
        public string tokenKey = "primary";
        public TokenType tokenType = TokenType.Color;
        public bool hasOverride;
        public T overrideValue;

        public T Resolve(Theme theme)
        {
            if (theme == null) return default;

            if (hasOverride)
                return overrideValue;

            return tokenType switch
            {
                TokenType.Color when typeof(T) == typeof(Color) => (T)(object)theme.GetColor(tokenKey),
                TokenType.Spacing when typeof(T) == typeof(float) => (T)(object)theme.GetSpacing(tokenKey),
                TokenType.Typography when typeof(T) == typeof(TypographyPreset) => (T)(object)theme.GetTypography(tokenKey),
                TokenType.Shape when typeof(T) == typeof(ShapePreset) => (T)(object)theme.GetShape(tokenKey),
                TokenType.Elevation when typeof(T) == typeof(ElevationPreset) => (T)(object)theme.GetElevation(tokenKey),
                _ => default
            };
        }
    }

    public enum TokenType
    {
        Color,
        Spacing,
        Typography,
        Shape,
        Elevation
    }
}
