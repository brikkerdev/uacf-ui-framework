using System.Collections;
using UnityEngine;
using UACF.UI.Theming;

namespace UACF.UI.Screens
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] private string screenId;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rootTransform;
        [SerializeField] private TransitionPreset showTransition;
        [SerializeField] private TransitionPreset hideTransition;

        protected bool isVisible;

        public string ScreenId => screenId;
        public RectTransform RootTransform => _rootTransform != null ? _rootTransform : transform as RectTransform;

        private void Awake()
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_rootTransform == null) _rootTransform = transform as RectTransform;
        }

        public virtual void OnScreenCreated() { }
        public virtual void OnScreenShow(object data) { }
        public virtual void OnScreenShown() { }
        public virtual void OnScreenHide() { }
        public virtual void OnScreenHidden() { }
        public virtual void OnScreenDestroy() { }
        public virtual void OnScreenFocus() { }
        public virtual void OnScreenBlur() { }
        public virtual bool OnBackPressed() => false;

        public void Show(object data = null, TransitionPreset transition = null)
        {
            var t = transition ?? showTransition;
            OnScreenShow(data);
            StartCoroutine(ShowCoroutine(t, data));
        }

        public void Hide(TransitionPreset transition = null)
        {
            var t = transition ?? hideTransition;
            OnScreenHide();
            StartCoroutine(HideCoroutine(t));
        }

        public void SetInteractable(bool interactable)
        {
            if (_canvasGroup != null)
                _canvasGroup.interactable = interactable;
        }

        private IEnumerator ShowCoroutine(TransitionPreset transition, object data)
        {
            gameObject.SetActive(true);
            if (_canvasGroup != null) _canvasGroup.alpha = 0f;

            if (transition != null)
                yield return transition.PlayShow(RootTransform, _canvasGroup);
            else if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            isVisible = true;
            OnScreenShown();
        }

        private IEnumerator HideCoroutine(TransitionPreset transition)
        {
            if (transition != null)
                yield return transition.PlayHide(RootTransform, _canvasGroup);
            else if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;

            isVisible = false;
            OnScreenHidden();
            gameObject.SetActive(false);
        }
    }
}
