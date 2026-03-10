using UnityEngine;
using UACF.UI.Tokens;

namespace UACF.UI.Styles
{
    public struct ResolvedStyle
    {
        public Color backgroundColor;
        public Color textColor;
        public TypographyPreset typography;
        public float borderRadius;
        public Color borderColor;
        public float borderWidth;
        public RectOffset padding;
        public float opacity;
        public ElevationPreset elevation;
        public bool hasBackground;
        public bool hasText;
        public bool hasBorder;
        public bool hasElevation;
    }
}
