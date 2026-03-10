using System;
using UnityEngine;
using UnityEngine.UI;

namespace UACF.UI.Styles
{
    [Serializable]
    public class UIStyleState
    {
        public OptionalToken<string> backgroundColorToken;
        public OptionalValue<Sprite> backgroundSprite;
        public OptionalValue<Image.Type> backgroundImageType;

        public OptionalToken<string> textColorToken;
        public OptionalToken<string> typographyToken;

        public OptionalToken<string> shapeToken;
        public OptionalToken<string> elevationToken;

        public OptionalToken<string> borderColorToken;
        public OptionalValue<float> borderWidth;

        public OptionalToken<string> paddingToken;
        public OptionalValue<RectOffset> paddingOverride;

        public OptionalValue<float> opacity;

        public OptionalValue<Vector2> preferredSize;
        public OptionalValue<Vector2> minSize;
        public OptionalValue<Vector2> maxSize;

        public OptionalValue<Vector3> scale;
        public OptionalValue<Vector3> rotation;
    }
}
