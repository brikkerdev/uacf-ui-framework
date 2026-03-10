using UnityEngine;

namespace UACF.UI.Tokens
{
    [System.Serializable]
    public class ShapePreset
    {
        public string key = "medium";
        public float borderRadius = 8f;
        public float borderWidth = 0f;
        public string borderColorToken = "divider";

        public ShapePreset() { }

        public ShapePreset(string key, float borderRadius, float borderWidth = 0f, string borderColorToken = "divider")
        {
            this.key = key;
            this.borderRadius = borderRadius;
            this.borderWidth = borderWidth;
            this.borderColorToken = borderColorToken;
        }
    }
}
