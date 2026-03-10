using UnityEngine;

namespace UACF.UI.Tokens
{
    [System.Serializable]
    public struct NamedColor
    {
        public string key;
        public Color color;
        [TextArea(1, 3)]
        public string description;

        public NamedColor(string key, Color color, string description = "")
        {
            this.key = key;
            this.color = color;
            this.description = description;
        }
    }
}
