using UnityEngine;

namespace UACF.UI.Tokens
{
    [System.Serializable]
    public struct NamedFloat
    {
        public string key;
        public float value;
        public string description;

        public NamedFloat(string key, float value, string description = "")
        {
            this.key = key;
            this.value = value;
            this.description = description;
        }
    }
}
