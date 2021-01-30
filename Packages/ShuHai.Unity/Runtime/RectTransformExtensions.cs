using UnityEngine;

namespace ShuHai.Unity
{
    public static class RectTransformExtensions
    {
        public static void SetSize(this RectTransform self, Vector2 size)
        {
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            self.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }
}
