using UnityEngine;
using UACF.UI.Tokens;

namespace UACF.UI.Styles
{
    public static class StyleResolver
    {
        public static Color ResolveColor(string token, Theme theme)
        {
            return theme != null ? theme.GetColor(token) : Color.magenta;
        }

        public static TypographyPreset ResolveTypography(string token, Theme theme)
        {
            return theme != null ? theme.GetTypography(token) : null;
        }

        public static float ResolveSpacing(string token, Theme theme)
        {
            return theme != null ? theme.GetSpacing(token) : 0f;
        }

        public static UIStyleState MergeStates(UIStyleState child, UIStyleState parent)
        {
            if (child == null) return parent;
            if (parent == null) return child;

            var merged = new UIStyleState();

            merged.backgroundColorToken = child.backgroundColorToken.hasValue ? child.backgroundColorToken : parent.backgroundColorToken;
            merged.backgroundSprite = child.backgroundSprite.hasValue ? child.backgroundSprite : parent.backgroundSprite;
            merged.backgroundImageType = child.backgroundImageType.hasValue ? child.backgroundImageType : parent.backgroundImageType;

            merged.textColorToken = child.textColorToken.hasValue ? child.textColorToken : parent.textColorToken;
            merged.typographyToken = child.typographyToken.hasValue ? child.typographyToken : parent.typographyToken;

            merged.shapeToken = child.shapeToken.hasValue ? child.shapeToken : parent.shapeToken;
            merged.elevationToken = child.elevationToken.hasValue ? child.elevationToken : parent.elevationToken;

            merged.borderColorToken = child.borderColorToken.hasValue ? child.borderColorToken : parent.borderColorToken;
            merged.borderWidth = child.borderWidth.hasValue ? child.borderWidth : parent.borderWidth;

            merged.paddingToken = child.paddingToken.hasValue ? child.paddingToken : parent.paddingToken;
            merged.paddingOverride = child.paddingOverride.hasValue ? child.paddingOverride : parent.paddingOverride;

            merged.opacity = child.opacity.hasValue ? child.opacity : parent.opacity;

            merged.preferredSize = child.preferredSize.hasValue ? child.preferredSize : parent.preferredSize;
            merged.minSize = child.minSize.hasValue ? child.minSize : parent.minSize;
            merged.maxSize = child.maxSize.hasValue ? child.maxSize : parent.maxSize;

            merged.scale = child.scale.hasValue ? child.scale : parent.scale;
            merged.rotation = child.rotation.hasValue ? child.rotation : parent.rotation;

            return merged;
        }

        public static ResolvedStyle Resolve(UIStyle style, UIComponentState state, Theme theme)
        {
            var resolved = new ResolvedStyle();

            if (style == null || theme == null)
                return resolved;

            var styleState = style.ResolveState(state);

            if (styleState.backgroundColorToken.hasValue)
            {
                resolved.backgroundColor = ResolveColor(styleState.backgroundColorToken.value, theme);
                resolved.hasBackground = true;
            }

            if (styleState.textColorToken.hasValue)
            {
                resolved.textColor = ResolveColor(styleState.textColorToken.value, theme);
                resolved.hasText = true;
            }

            if (styleState.typographyToken.hasValue)
            {
                resolved.typography = ResolveTypography(styleState.typographyToken.value, theme);
                resolved.hasText = true;
            }

            if (styleState.shapeToken.hasValue)
            {
                var shape = theme.GetShape(styleState.shapeToken.value);
                if (shape != null)
                {
                    resolved.borderRadius = shape.borderRadius;
                    resolved.borderWidth = shape.borderWidth;
                    resolved.borderColor = theme.GetColor(shape.borderColorToken);
                    resolved.hasBorder = shape.borderWidth > 0;
                }
            }

            if (styleState.elevationToken.hasValue)
            {
                resolved.elevation = theme.GetElevation(styleState.elevationToken.value);
                resolved.hasElevation = resolved.elevation != null && resolved.elevation.blur > 0;
            }

            if (styleState.paddingToken.hasValue)
                resolved.padding = theme.spacing != null ? theme.spacing.GetPadding(styleState.paddingToken.value) : new RectOffset();
            else if (styleState.paddingOverride.hasValue)
                resolved.padding = styleState.paddingOverride.value;

            resolved.opacity = styleState.opacity.hasValue ? styleState.opacity.value : 1f;

            return resolved;
        }
    }
}
