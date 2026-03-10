using UnityEngine;
using UnityEngine.EventSystems;
using UACF.UI.Animation;

namespace UACF.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class MicroInteraction : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        public bool scaleOnHover = true;
        public float hoverScale = 1.05f;
        public float hoverDuration = 0.15f;

        public bool scaleOnPress = true;
        public float pressScale = 0.95f;
        public float pressDuration = 0.1f;

        public bool colorOnHover = false;
        public string hoverColorToken;

        public bool soundOnClick = false;
        public AudioClip clickSound;

        public AnimationCurve curve;

        private RectTransform _rectTransform;
        private Vector3 _originalScale;
        private Coroutine _activeTween;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalScale = _rectTransform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (scaleOnHover)
            {
                CancelTween();
                _activeTween = UITween.ScaleTo(_rectTransform, _originalScale * hoverScale, hoverDuration, curve);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (scaleOnHover)
            {
                CancelTween();
                _activeTween = UITween.ScaleTo(_rectTransform, _originalScale, hoverDuration, curve);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (scaleOnPress)
            {
                CancelTween();
                _activeTween = UITween.ScaleTo(_rectTransform, _originalScale * pressScale, pressDuration, curve);
            }
            if (soundOnClick && clickSound != null)
            {
                var audioSource = GetComponent<UnityEngine.AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<UnityEngine.AudioSource>();
                audioSource.PlayOneShot(clickSound);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (scaleOnPress)
            {
                CancelTween();
                var targetScale = scaleOnHover ? _originalScale * hoverScale : _originalScale;
                _activeTween = UITween.ScaleTo(_rectTransform, targetScale, pressDuration, curve);
            }
        }

        private void CancelTween()
        {
            if (_activeTween != null)
            {
                StopCoroutine(_activeTween);
                _activeTween = null;
            }
        }
    }
}
