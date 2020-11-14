using UnityEngine;

namespace ShuHai.Unity
{
    public static class VectorExtensions
    {
        #region Verfication

        public static bool IsNaN(this Vector2 v) { return float.IsNaN(v.x) || float.IsNaN(v.y); }

        public static bool IsNaN(this Vector3 v) { return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z); }

        public static bool IsNaN(this Vector4 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z) || float.IsNaN(v.w);
        }

        public static bool IsInfinity(this Vector2 v) { return float.IsInfinity(v.x) || float.IsInfinity(v.y); }

        public static bool IsInfinity(this Vector3 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
        }

        public static bool IsInfinity(this Vector4 v)
        {
            return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z) || float.IsInfinity(v.w);
        }

        public static bool IsValid(this Vector2 v) { return !(IsNaN(v) || IsInfinity(v)); }

        public static bool IsValid(this Vector3 v) { return !(IsNaN(v) || IsInfinity(v)); }

        public static bool IsValid(this Vector4 v) { return !(IsNaN(v) || IsInfinity(v)); }

        #endregion Verfication

        #region Equality

        public static bool AlmostEquals(this Vector2 self, Vector2 other)
        {
            return AlmostEquals(self, other, Primitives.DefaultFloatTolerance);
        }

        public static bool AlmostEquals(this Vector2 self, Vector2 other, float tolerance)
        {
            return self.x.AlmostEquals(other.x, tolerance)
                   && self.y.AlmostEquals(other.y, tolerance);
        }

        public static bool AlmostEquals(this Vector3 self, Vector3 other)
        {
            return AlmostEquals(self, other, Primitives.DefaultFloatTolerance);
        }

        public static bool AlmostEquals(this Vector3 self, Vector3 other, float tolerance)
        {
            return self.x.AlmostEquals(other.x, tolerance)
                   && self.y.AlmostEquals(other.y, tolerance)
                   && self.z.AlmostEquals(other.z, tolerance);
        }

        public static bool AlmostEquals(this Vector4 self, Vector4 other)
        {
            return AlmostEquals(self, other, Primitives.DefaultFloatTolerance);
        }

        public static bool AlmostEquals(this Vector4 self, Vector4 other, float tolerance)
        {
            return self.x.AlmostEquals(other.x, tolerance)
                   && self.y.AlmostEquals(other.y, tolerance)
                   && self.z.AlmostEquals(other.z, tolerance)
                   && self.w.AlmostEquals(other.w, tolerance);
        }

        #endregion Equality

        #region Geometry Calculation

        public static float Cross(this Vector2 self, Vector2 other) { return self.x * other.y - self.y * other.x; }

        public static Vector3 Cross(this Vector3 self, Vector3 other) { return Vector3.Cross(self, other); }

        public static Vector2 Project(this Vector2 self, Vector2 on)
        {
            return on * Vector2.Dot(self, on) / Vector2.Dot(on, on);
        }

        public static Vector2 Rotate(this Vector2 self, float degrees)
        {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            var tx = self.x;
            var ty = self.y;
            self.x = cos * tx - sin * ty;
            self.y = sin * tx + cos * ty;
            return self;
        }

        public static Vector2 InverseScale(this Vector2 value, Vector2 scale)
        {
            if (scale.x != 0) scale.x = 1 / scale.x;
            if (scale.y != 0) scale.y = 1 / scale.y;
            return Vector2.Scale(value, scale);
        }

        public static Vector3 InverseScale(this Vector3 value, Vector3 scale)
        {
            if (scale.x != 0) scale.x = 1 / scale.x;
            if (scale.y != 0) scale.y = 1 / scale.y;
            if (scale.z != 0) scale.z = 1 / scale.z;
            return Vector3.Scale(value, scale);
        }

        #endregion Geometry Calculation
    }
}