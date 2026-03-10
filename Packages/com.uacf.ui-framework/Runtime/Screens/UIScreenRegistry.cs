using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Screens
{
    [Serializable]
    public class ScreenEntry
    {
        public string screenId;
        public GameObject prefab;
        public bool preload;
        public bool singleton;
    }

    [CreateAssetMenu(menuName = "UACF UI/Screen Registry", fileName = "ScreenRegistry")]
    public class UIScreenRegistry : ScriptableObject
    {
        public List<ScreenEntry> screens = new List<ScreenEntry>();

        public ScreenEntry GetScreen(string screenId)
        {
            foreach (var s in screens)
            {
                if (string.Equals(s.screenId, screenId, StringComparison.OrdinalIgnoreCase))
                    return s;
            }
            return null;
        }

        public bool HasScreen(string screenId) => GetScreen(screenId) != null;

        public List<string> GetAllScreenIds()
        {
            var list = new List<string>();
            foreach (var s in screens)
            {
                if (!string.IsNullOrEmpty(s.screenId))
                    list.Add(s.screenId);
            }
            return list;
        }
    }
}
