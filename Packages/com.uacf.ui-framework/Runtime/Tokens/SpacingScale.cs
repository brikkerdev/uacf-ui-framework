using System;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Tokens
{
    [CreateAssetMenu(menuName = "UACF UI/Tokens/Spacing Scale", fileName = "SpacingScale")]
    public class SpacingScale : ScriptableObject
    {
        public float none = 0f;
        public float xxs = 2f;
        public float xs = 4f;
        public float sm = 8f;
        public float md = 16f;
        public float lg = 24f;
        public float xl = 32f;
        public float xxl = 48f;
        public float xxxl = 64f;

        public List<NamedFloat> customSpacing = new List<NamedFloat>();

        private static readonly Dictionary<string, Func<SpacingScale, float>> ScaleGetters;

        static SpacingScale()
        {
            ScaleGetters = new Dictionary<string, Func<SpacingScale, float>>(StringComparer.OrdinalIgnoreCase)
            {
                { "none", s => s.none },
                { "xxs", s => s.xxs },
                { "xs", s => s.xs },
                { "sm", s => s.sm },
                { "md", s => s.md },
                { "lg", s => s.lg },
                { "xl", s => s.xl },
                { "xxl", s => s.xxl },
                { "xxxl", s => s.xxxl }
            };
        }

        public float GetSpacing(string key)
        {
            if (string.IsNullOrEmpty(key)) return 0f;

            if (ScaleGetters.TryGetValue(key, out var getter))
                return getter(this);

            var idx = customSpacing.FindIndex(s => string.Equals(s.key, key, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? customSpacing[idx].value : 0f;
        }

        public RectOffset GetPadding(string key)
        {
            var v = (int)GetSpacing(key);
            return new RectOffset(v, v, v, v);
        }

        public RectOffset GetPadding(string horizontal, string vertical)
        {
            var h = (int)GetSpacing(horizontal);
            var v = (int)GetSpacing(vertical);
            return new RectOffset(h, h, v, v);
        }

        public RectOffset GetPadding(string left, string right, string top, string bottom)
        {
            return new RectOffset(
                (int)GetSpacing(left),
                (int)GetSpacing(right),
                (int)GetSpacing(top),
                (int)GetSpacing(bottom)
            );
        }
    }
}
