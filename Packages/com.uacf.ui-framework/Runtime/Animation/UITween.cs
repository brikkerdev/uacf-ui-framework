using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UACF.UI.Animation
{
    public static class UITween
    {
        public static Coroutine To(MonoBehaviour owner, float from, float to, float duration,
            Action<float> onUpdate, AnimationCurve curve = null, Action onComplete = null)
        {
            return owner.StartCoroutine(ToCoroutine(from, to, duration, onUpdate, curve, onComplete));
        }

        public static Coroutine FadeTo(CanvasGroup target, float alpha, float duration, AnimationCurve curve = null)
        {
            if (target == null) return null;
            var mb = target.GetComponent<MonoBehaviour>();
            return mb != null ? mb.StartCoroutine(ToCoroutine(target.alpha, alpha, duration, v => target.alpha = v, curve, null)) : null;
        }

        public static Coroutine MoveTo(RectTransform target, Vector2 position, float duration, AnimationCurve curve = null)
        {
            if (target == null) return null;
            var mb = target.GetComponent<MonoBehaviour>();
            var from = target.anchoredPosition;
            return mb != null ? mb.StartCoroutine(ToCoroutine(0f, 1f, duration,
                t => target.anchoredPosition = Vector2.Lerp(from, position, t), curve, null)) : null;
        }

        public static Coroutine ScaleTo(RectTransform target, Vector3 scale, float duration, AnimationCurve curve = null)
        {
            if (target == null) return null;
            var mb = target.GetComponent<MonoBehaviour>();
            var from = target.localScale;
            return mb != null ? mb.StartCoroutine(ToCoroutine(0f, 1f, duration,
                t => target.localScale = Vector3.Lerp(from, scale, t), curve, null)) : null;
        }

        public static Coroutine ColorTo(Graphic target, Color color, float duration, AnimationCurve curve = null)
        {
            if (target == null) return null;
            var mb = target.GetComponent<MonoBehaviour>();
            var from = target.color;
            return mb != null ? mb.StartCoroutine(ToCoroutine(0f, 1f, duration,
                t => target.color = Color.Lerp(from, color, t), curve, null)) : null;
        }

        public static Coroutine SizeTo(RectTransform target, Vector2 size, float duration, AnimationCurve curve = null)
        {
            if (target == null) return null;
            var mb = target.GetComponent<MonoBehaviour>();
            var from = target.sizeDelta;
            return mb != null ? mb.StartCoroutine(ToCoroutine(0f, 1f, duration,
                t => target.sizeDelta = Vector2.Lerp(from, size, t), curve, null)) : null;
        }

        public static void Cancel(Coroutine tween)
        {
            // Caller must track and stop coroutine - we don't have access to the owner here
        }

        public static void CancelAll(MonoBehaviour owner)
        {
            if (owner != null)
                owner.StopAllCoroutines();
        }

        private static IEnumerator ToCoroutine(float from, float to, float duration,
            Action<float> onUpdate, AnimationCurve curve, Action onComplete)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = curve != null ? curve.Evaluate(t) : t;
                var value = Mathf.Lerp(from, to, eased);
                onUpdate?.Invoke(value);
                yield return null;
            }
            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
    }
}
