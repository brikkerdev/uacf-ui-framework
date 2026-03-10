using UnityEngine;

namespace UACF.UI.Tokens
{
    [System.Serializable]
    public class ElevationPreset
    {
        public string key = "none";
        public float offsetX = 0f;
        public float offsetY = 0f;
        public float blur = 0f;
        public float spread = 0f;
        public string shadowColorToken = "shadow";
        public float shadowAlpha = 0.12f;

        public ElevationPreset() { }

        public ElevationPreset(string key, float offsetY, float blur, float shadowAlpha = 0.12f)
        {
            this.key = key;
            this.offsetY = offsetY;
            this.blur = blur;
            this.shadowAlpha = shadowAlpha;
        }
    }
}
