using UnityEngine;

namespace ShuHai.Unity.Demo
{
    public class CurveView<TCurve, TSpline> : MonoBehaviour
        where TSpline : ISpline, new()
        where TCurve : SplineCurve<TSpline>
    {
        public bool ShowSegments = true;
        public bool ShowPoints = true;
        public bool ShowKeyPoints = true;

        public Color[] SegmentColors = { Color.cyan, Color.green };

        public float PointSize = 0.2f;

        public TCurve Curve;

        protected void OnDrawGizmos()
        {
            if (Curve == null)
                return;

            for (int s = 0; s < Curve.SegmentCount; ++s)
            {
                var segment = Curve.GetSegment(s);
                var points = segment.Points;

                if (ShowSegments)
                {
                    using (new GizmosColorScope(SegmentColors.LoopedAt(s, Color.cyan)))
                    {
                        for (int i = 0; i < points.Count - 1; ++i)
                        {
                            var p0 = points[i];
                            var p1 = points[i + 1];
                            GizmosEx.DrawLine(p0, p1, transform);
                        }
                    }
                }

                if (ShowPoints)
                {
                    using (new GizmosColorScope(Color.yellow))
                    {
                        foreach (var point in points)
                            GizmosEx.DrawCross(point, PointSize, transform);
                    }
                }
            }

            if (ShowKeyPoints)
            {
                using (new GizmosColorScope(Color.magenta))
                {
                    foreach (var point in Curve.KeyPoints)
                        GizmosEx.DrawCross(point, PointSize, transform);
                }
            }
        }
    }
}