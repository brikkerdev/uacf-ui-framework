using System.Collections;
using UnityEngine;

namespace UACF.UI.Screens
{
    public enum TransitionType
    {
        None,
        Fade,
        SlideLeft,
        SlideRight,
        SlideUp,
        SlideDown,
        Scale,
        ScaleAndFade,
        Custom
    }

    [CreateAssetMenu(menuName = "UACF UI/Transition Preset", fileName = "TransitionPreset")]
    public class TransitionPreset : ScriptableObject
    {
        public TransitionType type = TransitionType.Fade;
        public float duration = 0.3f;
        public AnimationCurve curve;
        public float delay;

        public IEnumerator PlayShow(RectTransform target, CanvasGroup canvasGroup)
        {
            if (delay > 0) yield return new WaitForSecondsRealtime(delay);

            switch (type)
            {
                case TransitionType.Fade:
                    yield return Fade(canvasGroup, 0f, 1f);
                    break;
                case TransitionType.SlideLeft:
                    yield return SlideAndFade(target, canvasGroup, new Vector2(Screen.width, 0), Vector2.zero, 0f, 1f);
                    break;
                case TransitionType.SlideRight:
                    yield return SlideAndFade(target, canvasGroup, new Vector2(-Screen.width, 0), Vector2.zero, 0f, 1f);
                    break;
                case TransitionType.SlideUp:
                    yield return SlideAndFade(target, canvasGroup, new Vector2(0, -Screen.height), Vector2.zero, 0f, 1f);
                    break;
                case TransitionType.SlideDown:
                    yield return SlideAndFade(target, canvasGroup, new Vector2(0, Screen.height), Vector2.zero, 0f, 1f);
                    break;
                case TransitionType.Scale:
                case TransitionType.ScaleAndFade:
                    yield return ScaleAndFade(target, canvasGroup, Vector3.one * 0.8f, Vector3.one, 0f, 1f);
                    break;
                default:
                    if (canvasGroup != null) canvasGroup.alpha = 1f;
                    break;
            }
        }

        public IEnumerator PlayHide(RectTransform target, CanvasGroup canvasGroup)
        {
            if (delay > 0) yield return new WaitForSecondsRealtime(delay);

            switch (type)
            {
                case TransitionType.Fade:
                    yield return Fade(canvasGroup, 1f, 0f);
                    break;
                case TransitionType.SlideLeft:
                    yield return SlideAndFade(target, canvasGroup, Vector2.zero, new Vector2(-Screen.width, 0), 1f, 0f);
                    break;
                case TransitionType.SlideRight:
                    yield return SlideAndFade(target, canvasGroup, Vector2.zero, new Vector2(Screen.width, 0), 1f, 0f);
                    break;
                case TransitionType.SlideUp:
                    yield return SlideAndFade(target, canvasGroup, Vector2.zero, new Vector2(0, Screen.height), 1f, 0f);
                    break;
                case TransitionType.SlideDown:
                    yield return SlideAndFade(target, canvasGroup, Vector2.zero, new Vector2(0, -Screen.height), 1f, 0f);
                    break;
                case TransitionType.Scale:
                case TransitionType.ScaleAndFade:
                    yield return ScaleAndFade(target, canvasGroup, Vector3.one, Vector3.one * 0.8f, 1f, 0f);
                    break;
                default:
                    if (canvasGroup != null) canvasGroup.alpha = 0f;
                    break;
            }
        }

        private IEnumerator Fade(CanvasGroup cg, float from, float to)
        {
            if (cg == null) yield break;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = curve != null ? curve.Evaluate(elapsed / duration) : elapsed / duration;
                cg.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            cg.alpha = to;
        }

        private IEnumerator SlideAndFade(RectTransform target, CanvasGroup cg, Vector2 from, Vector2 to, float alphaFrom, float alphaTo)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = curve != null ? curve.Evaluate(elapsed / duration) : elapsed / duration;
                if (target != null) target.anchoredPosition = Vector2.Lerp(from, to, t);
                if (cg != null) cg.alpha = Mathf.Lerp(alphaFrom, alphaTo, t);
                yield return null;
            }
            if (target != null) target.anchoredPosition = to;
            if (cg != null) cg.alpha = alphaTo;
        }

        private IEnumerator ScaleAndFade(RectTransform target, CanvasGroup cg, Vector3 from, Vector3 to, float alphaFrom, float alphaTo)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = curve != null ? curve.Evaluate(elapsed / duration) : elapsed / duration;
                if (target != null) target.localScale = Vector3.Lerp(from, to, t);
                if (cg != null) cg.alpha = Mathf.Lerp(alphaFrom, alphaTo, t);
                yield return null;
            }
            if (target != null) target.localScale = to;
            if (cg != null) cg.alpha = alphaTo;
        }
    }
}
