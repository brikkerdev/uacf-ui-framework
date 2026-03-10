using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Styles
{
    [CreateAssetMenu(menuName = "UACF UI/Style Sheet", fileName = "StyleSheet")]
    public class StyleSheet : ScriptableObject
    {
        public List<UIStyle> styles = new List<UIStyle>();

        public UIStyle GetStyle(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            foreach (var s in styles)
            {
                if (s != null && string.Equals(s.styleKey, key, System.StringComparison.OrdinalIgnoreCase))
                    return s;
            }
            return null;
        }

        public bool TryGetStyle(string key, out UIStyle style)
        {
            style = GetStyle(key);
            return style != null;
        }

        public List<string> GetAllKeys()
        {
            var list = new List<string>();
            foreach (var s in styles)
            {
                if (s != null && !string.IsNullOrEmpty(s.styleKey))
                    list.Add(s.styleKey);
            }
            return list;
        }

        public void AddStyle(UIStyle style)
        {
            if (style != null && !styles.Contains(style))
                styles.Add(style);
        }

        public void RemoveStyle(string key)
        {
            styles.RemoveAll(s => s != null && string.Equals(s.styleKey, key, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
