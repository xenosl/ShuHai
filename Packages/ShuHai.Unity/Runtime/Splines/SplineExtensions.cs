using System;
using UnityEngine;

namespace ShuHai.Unity
{
    public static class SplineExtensions
    {
        #region Points

        /// <summary>
        ///     Get the auto-generated head control point of current spline.
        /// </summary>
        /// <remarks>
        ///     At least 4 control points is required to form a spline, the first and last control point are automatically
        ///     generated if control points defined by user is insufficient.
        /// </remarks>
        public static Vector3 GetExtraHeadPoint(this ISpline self)
        {
            return self.PointCount >= 2 ? self[0] + (self[0] - self[1]) : Vector3.zero;
        }

        /// <summary>
        ///     Get the auto-generated tail control point of current spline.
        /// </summary>
        /// <remarks>
        ///     At least 4 control points is required to form a spline, the head and tail control point are automatically
        ///     generated if control points defined by user is insufficient.
        /// </remarks>
        public static Vector3 GetExtraTailPoint(this ISpline self)
        {
            var pc = self.PointCount;
            return pc >= 2 ? self[pc - 1] + (self[pc - 1] - self[pc - 2]) : Vector3.zero;
        }

        #endregion Points

        #region Segments

        /// <summary>
        ///     Get the segment between each adjacent control points of current spline.
        /// </summary>
        /// <param name="self">The spline instance.</param>
        /// <param name="index">
        ///     Index of the segment. The value can be -1 which means the segment formed by the extra head control point
        ///     and the first control point of the spline; and <see cref="ISpline.PointCount" />-1 which means the segment
        ///     formed by the last control point of the spline and extra tail control point.
        /// </param>
        public static Segment3 GetSegment(this ISpline self, int index)
        {
            var pc = self.PointCount;
            if (index < -1 || index > pc - 1)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (index == -1)
                return new Segment3(self.GetExtraHeadPoint(), self[0]);
            if (index == pc - 1)
                return new Segment3(self[pc - 1], self.GetExtraTailPoint());
            return new Segment3(self[index], self[index + 1]);
        }

        #endregion Segments

        #region Interpolation

        /// <summary>
        ///     Interpolates over the whole spline by the specified parametric value.
        /// </summary>
        /// <param name="self">The spline instance applied.</param>
        /// <param name="t">
        ///     Parametric value ranges from 0 to 1 representing the position along the whole length of the spline.
        /// </param>
        public static Vector3 Interpolate(this ISpline self, float t)
        {
            float pos = t * (self.PointCount - 1);
            var index = (int)pos;
            return self.Interpolate(index, pos - index);
        }

        internal static Vector3 Interpolate(this ISpline self,
            Matrix4x4 coefficient, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float tt = t * t, ttt = tt * t;
            var powers = new Vector4(ttt, tt, t, 1);

            var cm = new Matrix4x4(); // Control Matrix
            cm.SetRow(0, p0, 1);
            cm.SetRow(1, p1, 1);
            cm.SetRow(2, p2, 1);
            cm.SetRow(3, p3, 1);

            // P(t) = powers * Coefficient * Matrix4(p1, p2, tangent1, tangent2)
            return (coefficient * cm).transpose * powers;
        }

        #endregion Interpolation
    }
}