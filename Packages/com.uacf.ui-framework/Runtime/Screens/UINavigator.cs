using System.Collections;
using UnityEngine;

namespace UACF.UI.Screens
{
    [RequireComponent(typeof(RectTransform))]
    public class UINavigator : MonoBehaviour
    {
        [SerializeField] private UIScreenRegistry _registry;
        [SerializeField] private RectTransform _screenContainer;
        [SerializeField] private TransitionPreset _defaultTransition;

        private readonly NavigationStack _stack = new NavigationStack();
        private UIScreen _currentScreen;

        public UIScreen CurrentScreen => _currentScreen;
        public int StackDepth => _stack.Count;
        public string CurrentScreenId => _currentScreen?.ScreenId;

        public void Push(string screenId, object data = null, TransitionPreset transition = null)
        {
            StartCoroutine(PushCoroutine(screenId, data, transition));
        }

        public void Pop(object result = null, TransitionPreset transition = null)
        {
            StartCoroutine(PopCoroutine(transition));
        }

        public void Replace(string screenId, object data = null)
        {
            StartCoroutine(ReplaceCoroutine(screenId, data));
        }

        public void PopToRoot()
        {
            while (_stack.Count > 1)
            {
                var entry = _stack.Pop();
                if (entry.instance != null)
                    entry.instance.Hide(null);
            }
        }

        public void PopAll()
        {
            while (_stack.Count > 0)
            {
                var entry = _stack.Pop();
                if (entry.instance != null)
                    entry.instance.Hide(null);
            }
            _currentScreen = null;
        }

        public bool CanGoBack() => _stack.Count > 1;

        private IEnumerator PushCoroutine(string screenId, object data, TransitionPreset transition)
        {
            var entry = _registry?.GetScreen(screenId);
            if (entry == null || entry.prefab == null)
            {
                Debug.LogError($"[UINavigator] Screen '{screenId}' not found in registry.");
                yield break;
            }

            if (_currentScreen != null)
                _currentScreen.OnScreenBlur();

            var go = Instantiate(entry.prefab, _screenContainer);
            var screen = go.GetComponent<UIScreen>();
            if (screen == null)
                screen = go.AddComponent<UIScreen>();

            var t = transition ?? _defaultTransition;
            screen.Show(data, t);
            _stack.Push(new NavigationEntry(screenId, screen, data));
            _currentScreen = screen;
        }

        private IEnumerator PopCoroutine(TransitionPreset transition)
        {
            var entry = _stack.Pop();
            if (entry?.instance != null)
            {
                entry.instance.Hide(transition ?? _defaultTransition);
                yield return new WaitForSecondsRealtime(0.35f);
            }

            var prev = _stack.Peek();
            _currentScreen = prev?.instance;
            _currentScreen?.OnScreenFocus();
        }

        private IEnumerator ReplaceCoroutine(string screenId, object data)
        {
            yield return PopCoroutine(null);
            yield return PushCoroutine(screenId, data, null);
        }
    }
}
