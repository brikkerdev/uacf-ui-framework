using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [System.Serializable]
    public struct IconEntry
    {
        public string key;
        public Sprite sprite;
        public string description;
    }

    [CreateAssetMenu(menuName = "UACF UI/Tokens/Icon Set", fileName = "IconSet")]
    public class IconSet : ScriptableObject
    {
        public List<IconEntry> icons = new List<IconEntry>();

        public Sprite GetIcon(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            var idx = icons.FindIndex(i => string.Equals(i.key, key, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? icons[idx].sprite : null;
        }

        public bool TryGetIcon(string key, out Sprite sprite)
        {
            sprite = GetIcon(key);
            return sprite != null;
        }

        public void SetIcon(string key, Sprite sprite)
        {
            var idx = icons.FindIndex(i => string.Equals(i.key, key, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0)
                icons[idx] = new IconEntry { key = key, sprite = sprite, description = icons[idx].description };
            else
                icons.Add(new IconEntry { key = key, sprite = sprite });
        }

        public List<string> GetAllIconKeys()
        {
            var list = new List<string>();
            foreach (var i in icons)
            {
                if (!string.IsNullOrEmpty(i.key))
                    list.Add(i.key);
            }
            return list;
        }
    }
}
