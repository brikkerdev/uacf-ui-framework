using UnityEngine;

namespace UACF.UI.Utility
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private bool applyTop = true;
        [SerializeField] private bool applyBottom = true;
        [SerializeField] private bool applyLeft = true;
        [SerializeField] private bool applyRight = true;

        private void Awake()
        {
            if (_panel == null) _panel = GetComponent<RectTransform>();
        }

        private void Update()
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (_panel == null) return;

            var safeArea = Screen.safeArea;
            var size = _panel.rect.size;
            var anchorMin = _panel.anchorMin;
            var anchorMax = _panel.anchorMax;

            if (applyLeft)
                anchorMin.x = safeArea.xMin / Screen.width;
            if (applyRight)
                anchorMax.x = safeArea.xMax / Screen.width;
            if (applyBottom)
                anchorMin.y = safeArea.yMin / Screen.height;
            if (applyTop)
                anchorMax.y = safeArea.yMax / Screen.height;

            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;
        }
    }
}
