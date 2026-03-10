using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Tokens/Elevation Set", fileName = "ElevationSet")]
    public class ElevationSet : ScriptableObject
    {
        public ElevationPreset none = new ElevationPreset("none", 0f, 0f, 0f);
        public ElevationPreset low = new ElevationPreset("low", 1f, 3f, 0.12f);
        public ElevationPreset mid = new ElevationPreset("mid", 3f, 6f, 0.16f);
        public ElevationPreset high = new ElevationPreset("high", 6f, 12f, 0.24f);

        public List<ElevationPreset> customPresets = new List<ElevationPreset>();

        private static readonly Dictionary<string, Func<ElevationSet, ElevationPreset>> PresetGetters;

        static ElevationSet()
        {
            PresetGetters = new Dictionary<string, Func<ElevationSet, ElevationPreset>>(StringComparer.OrdinalIgnoreCase)
            {
                { "none", s => s.none },
                { "low", s => s.low },
                { "mid", s => s.mid },
                { "high", s => s.high }
            };
        }

        public ElevationPreset GetPreset(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (PresetGetters.TryGetValue(key, out var getter))
                return getter(this);

            var idx = customPresets.FindIndex(p => p != null && string.Equals(p.key, key, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? customPresets[idx] : null;
        }
    }
}
