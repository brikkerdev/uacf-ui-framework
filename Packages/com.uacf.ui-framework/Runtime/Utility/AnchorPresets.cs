using UnityEngine;

namespace UACF.UI.Utility
{
    public static class AnchorPresets
    {
        public static void StretchFull(RectTransform rt)
        {
            if (rt == null) return;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static void TopStretch(RectTransform rt, float height)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.offsetMin = new Vector2(0, -height);
            rt.offsetMax = new Vector2(0, 0);
        }

        public static void BottomStretch(RectTransform rt, float height)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, height);
        }

        public static void CenterMiddle(RectTransform rt, Vector2 size)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
        }

        public static void TopLeft(RectTransform rt, Vector2 size)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.sizeDelta = size;
        }

        public static void TopRight(RectTransform rt, Vector2 size)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.sizeDelta = size;
        }

        public static void BottomLeft(RectTransform rt, Vector2 size)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0, 0);
            rt.sizeDelta = size;
        }

        public static void BottomRight(RectTransform rt, Vector2 size)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(1, 0);
            rt.sizeDelta = size;
        }

        public static void LeftStretch(RectTransform rt, float width)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 0.5f);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(width, 0);
        }

        public static void RightStretch(RectTransform rt, float width)
        {
            if (rt == null) return;
            rt.anchorMin = new Vector2(1, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 0.5f);
            rt.offsetMin = new Vector2(-width, 0);
            rt.offsetMax = new Vector2(0, 0);
        }
    }
}
