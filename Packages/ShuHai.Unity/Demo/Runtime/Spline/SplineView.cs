using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Demo
{
    [ExecuteInEditMode]
    public abstract class SplineView : MonoBehaviour
    {
        protected ISpline Spline;

        protected abstract ISpline CreateSpline();

        protected void Reset()
        {
            Spline = CreateSpline();
            foreach (var point in transform.Cast<Transform>().Select(t => t.localPosition))
                Spline.AddPoint(point);
        }

        private void OnEnable() { Reset(); }
        private void OnDisable() { Spline = null; }

        private void Update()
        {
            if (transform.childCount != Spline.PointCount)
                Reset();
        }

        internal void OnPointTransformChanged(SplinePoint point)
        {
            var pt = point.transform;
            int index = pt.GetSiblingIndex();
            Spline[index] = pt.localPosition;
        }

        #region Gizmos

        public const int MinGizmoSegmentCount = 8;
        public const int MaxGizmoSegmentCount = 128;

        public int GizmoSegmentCount
        {
            get => _gizmoSegmentCount;
            set => _gizmoSegmentCount = Mathf.Clamp(value, MinGizmoSegmentCount, MaxGizmoSegmentCount);
        }

        [SerializeField, Range(MinGizmoSegmentCount, MaxGizmoSegmentCount)]
        private int _gizmoSegmentCount = 64;

        protected virtual void OnDrawGizmos()
        {
            if (Spline == null)
                return;

            using (new GizmosColorScope(Color.cyan))
            {
                var unit = 1f / GizmoSegmentCount;
                for (int i = 0; i < GizmoSegmentCount; ++i)
                {
                    var p0 = Spline.Interpolate(unit * i);
                    var p1 = Spline.Interpolate(unit * (i + 1));
                    GizmosEx.DrawLine(p0, p1, transform);
                }
            }
        }

        #endregion Gizmos
    }
}
