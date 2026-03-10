using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Tokens/Shape Set", fileName = "ShapeSet")]
    public class ShapeSet : ScriptableObject
    {
        public ShapePreset none = new ShapePreset("none", 0f);
        public ShapePreset small = new ShapePreset("small", 4f);
        public ShapePreset medium = new ShapePreset("medium", 8f);
        public ShapePreset large = new ShapePreset("large", 16f);
        public ShapePreset extraLarge = new ShapePreset("extraLarge", 24f);
        public ShapePreset circle = new ShapePreset("circle", 9999f);

        public List<ShapePreset> customPresets = new List<ShapePreset>();

        private static readonly Dictionary<string, Func<ShapeSet, ShapePreset>> PresetGetters;

        static ShapeSet()
        {
            PresetGetters = new Dictionary<string, Func<ShapeSet, ShapePreset>>(StringComparer.OrdinalIgnoreCase)
            {
                { "none", s => s.none },
                { "small", s => s.small },
                { "medium", s => s.medium },
                { "large", s => s.large },
                { "extraLarge", s => s.extraLarge },
                { "circle", s => s.circle }
            };
        }

        public ShapePreset GetPreset(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            if (PresetGetters.TryGetValue(key, out var getter))
                return getter(this);

            var idx = customPresets.FindIndex(p => p != null && string.Equals(p.key, key, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? customPresets[idx] : null;
        }
    }
}
