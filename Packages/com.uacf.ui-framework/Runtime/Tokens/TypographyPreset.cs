using System;
using TMPro;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [Serializable]
    public class TypographyPreset
    {
        public string key = "body1";
        public TMP_FontAsset font;
        public FontStyles fontStyle = FontStyles.Normal;
        public float fontSize = 16f;
        public float lineSpacing = 1.2f;
        public float characterSpacing = 0f;
        public float paragraphSpacing = 0f;
        public TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft;
        public bool autoSize = false;
        public float autoSizeMin = 12f;
        public float autoSizeMax = 72f;
        public TextOverflowModes overflow = TextOverflowModes.Overflow;

        public TypographyPreset() { }

        public TypographyPreset(string key, float fontSize, FontStyles style = FontStyles.Normal)
        {
            this.key = key;
            this.fontSize = fontSize;
            this.fontStyle = style;
        }

        public void ApplyTo(TMP_Text text)
        {
            if (text == null) return;
            if (font != null) text.font = font;
            text.fontStyle = fontStyle;
            text.fontSize = fontSize;
            text.lineSpacing = lineSpacing;
            text.characterSpacing = characterSpacing;
            text.paragraphSpacing = paragraphSpacing;
            text.alignment = alignment;
            text.enableAutoSizing = autoSize;
            text.fontSizeMin = autoSizeMin;
            text.fontSizeMax = autoSizeMax;
            text.overflowMode = overflow;
        }
    }
}
