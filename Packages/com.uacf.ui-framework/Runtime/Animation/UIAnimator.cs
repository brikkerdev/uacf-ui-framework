using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UACF.UI.Animation
{
    public enum AnimationType
    {
        Fade,
        Slide,
        Scale,
        Rotate,
        Bounce,
        Shake,
        Pulse
    }

    [Serializable]
    public class NamedAnimation
    {
        public string key = "show";
        public AnimationType type;
        public TweenPreset preset;
        public Vector2 moveOffset;
        public Vector3 scaleFrom;
        public float fadeFrom;
    }

    [RequireComponent(typeof(RectTransform))]
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TweenPreset defaultPreset;
        [SerializeField] private List<NamedAnimation> animations = new List<NamedAnimation>();

        private void Awake()
        {
            if (_target == null) _target = GetComponent<RectTransform>();
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        }

        public Coroutine Play(string key)
        {
            if (animations == null) return null;
            foreach (var a in animations)
            {
                if (string.Equals(a.key, key, StringComparison.OrdinalIgnoreCase))
                    return StartCoroutine(PlayAnimation(a));
            }
            return null;
        }

        public Coroutine PlayShow()
        {
            return Play("show");
        }

        public Coroutine PlayHide()
        {
            return Play("hide");
        }

        public void StopAll()
        {
            StopAllCoroutines();
        }

        private IEnumerator PlayAnimation(NamedAnimation anim)
        {
            var preset = anim.preset ?? defaultPreset;
            var duration = preset != null ? preset.duration : 0.3f;
            var curve = preset != null ? preset.curve : null;

            switch (anim.type)
            {
                case AnimationType.Fade:
                    if (_canvasGroup != null)
                    {
                        var from = _canvasGroup.alpha;
                        var elapsed = 0f;
                        while (elapsed < duration)
                        {
                            elapsed += Time.unscaledDeltaTime;
                            var t = curve != null ? curve.Evaluate(elapsed / duration) : elapsed / duration;
                            _canvasGroup.alpha = Mathf.Lerp(from, anim.fadeFrom, t);
                            yield return null;
                        }
                        _canvasGroup.alpha = anim.fadeFrom;
                    }
                    break;
                case AnimationType.Scale:
                    if (_target != null)
                    {
                        UITween.ScaleTo(_target, anim.scaleFrom, duration, curve);
                        yield return new WaitForSecondsRealtime(duration);
                    }
                    break;
                default:
                    yield break;
            }
        }
    }
}
