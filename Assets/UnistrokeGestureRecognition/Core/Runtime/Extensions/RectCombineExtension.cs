using UnityEngine;

namespace UnistrokeGestureRecognition {
    internal static class RectCombineExtension {
        public static Rect Combine(this Rect rect, Rect other) {
            return Rect.MinMaxRect(
                Mathf.Min(rect.xMin, other.xMin),
                Mathf.Min(rect.yMin, other.yMin),
                Mathf.Max(rect.xMax, other.xMax),
                Mathf.Max(rect.yMax, other.yMax));
        }
    }
}