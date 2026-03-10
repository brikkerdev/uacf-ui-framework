using UnityEngine;
using UnityEngine.UI;

namespace UACF.UI.Utility
{
    public static class UIExtensions
    {
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            if (graphic == null) return;
            var c = graphic.color;
            c.a = Mathf.Clamp01(alpha);
            graphic.color = c;
        }

        public static void SetAlpha(this CanvasGroup group, float alpha)
        {
            if (group == null) return;
            group.alpha = Mathf.Clamp01(alpha);
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }

        public static void DestroyChildren(this Transform transform)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (Application.isPlaying)
                    Object.Destroy(child.gameObject);
                else
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        public static void SetActive(this CanvasGroup group, bool active)
        {
            if (group == null) return;
            group.alpha = active ? 1f : 0f;
            group.interactable = active;
            group.blocksRaycasts = active;
        }

        public static Rect GetScreenRect(this RectTransform rt)
        {
            if (rt == null) return default;
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }

        public static void SetPivotWithoutMoving(this RectTransform rt, Vector2 pivot)
        {
            if (rt == null) return;
            var size = rt.rect.size;
            var delta = pivot - rt.pivot;
            rt.pivot = pivot;
            rt.anchoredPosition += new Vector2(delta.x * size.x, delta.y * size.y);
        }
    }
}
