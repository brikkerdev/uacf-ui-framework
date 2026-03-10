using UnityEngine;

namespace UACF.UI.Animation
{
    public static class EasingFunctions
    {
        public static float Linear(float t) => t;

        public static float EaseInQuad(float t) => t * t;
        public static float EaseOutQuad(float t) => t * (2f - t);
        public static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseOutCubic(float t) => 1f + (--t) * t * t;
        public static float EaseInOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f + (--t) * (2f * (--t)) * (2f * t);

        public static float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }

        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        public static float EaseInOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
        }

        public static float EaseInElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * (2f * Mathf.PI) / 3f);
        }

        public static float EaseOutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI) / 3f) + 1f;
        }

        public static float EaseOutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
                return n1 * t * t;
            if (t < 2f / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static float Spring(float t)
        {
            return 1f - Mathf.Cos(t * Mathf.PI * 4f) * (1f - t);
        }

        public static AnimationCurve EaseInOutCurve()
        {
            return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }

        public static AnimationCurve EaseOutBackCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 0f),
                new Keyframe(0.5f, 0.9f, 1f, 1f),
                new Keyframe(1f, 1f, 2f, 0f)
            );
        }

        public static AnimationCurve SpringCurve()
        {
            return new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 0f),
                new Keyframe(0.5f, 1.2f, 0f, 0f),
                new Keyframe(0.75f, 0.95f, 0f, 0f),
                new Keyframe(1f, 1f, 0f, 0f)
            );
        }

        public static float Evaluate(float t, AnimationCurve curve)
        {
            return curve != null ? curve.Evaluate(t) : t;
        }
    }
}
