using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Tokens/Typography Set", fileName = "TypographySet")]
    public class TypographySet : ScriptableObject
    {
        public TypographyPreset h1 = new TypographyPreset("h1", 32f, FontStyles.Bold);
        public TypographyPreset h2 = new TypographyPreset("h2", 24f, FontStyles.Bold);
        public TypographyPreset h3 = new TypographyPreset("h3", 20f, FontStyles.Bold);
        public TypographyPreset h4 = new TypographyPreset("h4", 18f, FontStyles.Bold);
        public TypographyPreset subtitle1 = new TypographyPreset("subtitle1", 16f, FontStyles.Normal);
        public TypographyPreset subtitle2 = new TypographyPreset("subtitle2", 14f, FontStyles.Normal);
        public TypographyPreset body1 = new TypographyPreset("body1", 16f);
        public TypographyPreset body2 = new TypographyPreset("body2", 14f);
        public TypographyPreset caption = new TypographyPreset("caption", 12f);
        public TypographyPreset overline = new TypographyPreset("overline", 10f, FontStyles.Normal);
        public TypographyPreset button = new TypographyPreset("button", 14f, FontStyles.Bold);

        public List<TypographyPreset> customPresets = new List<TypographyPreset>();

        private static readonly Dictionary<string, Func<TypographySet, TypographyPreset>> PresetGetters;

        static TypographySet()
        {
            PresetGetters = new Dictionary<string, Func<TypographySet, TypographyPreset>>(StringComparer.OrdinalIgnoreCase)
            {
                { "h1", s => s.h1 },
                { "h2", s => s.h2 },
                { "h3", s => s.h3 },
                { "h4", s => s.h4 },
                { "subtitle1", s => s.subtitle1 },
                { "subtitle2", s => s.subtitle2 },
                { "body1", s => s.body1 },
                { "body2", s => s.body2 },
                { "caption", s => s.caption },
                { "overline", s => s.overline },
                { "button", s => s.button }
            };
        }

        public TypographyPreset GetPreset(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (PresetGetters.TryGetValue(key, out var getter))
                return getter(this);

            var idx = customPresets.FindIndex(p => string.Equals(p?.key, key, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? customPresets[idx] : null;
        }

        public void ApplyTo(TMP_Text text, string presetKey)
        {
            var preset = GetPreset(presetKey);
            preset?.ApplyTo(text);
        }

        public List<string> GetAllPresetKeys()
        {
            var list = new List<string>(PresetGetters.Keys);
            foreach (var p in customPresets)
            {
                if (p != null && !string.IsNullOrEmpty(p.key) && !list.Contains(p.key))
                    list.Add(p.key);
            }
            return list;
        }
    }
}
