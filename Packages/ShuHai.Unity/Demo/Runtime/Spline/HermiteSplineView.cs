using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Demo
{
    [ExecuteInEditMode]
    public class HermiteSplineView : SplineView
    {
        protected new HermiteSpline Spline => (HermiteSpline)base.Spline;

        protected override ISpline CreateSpline() { return new HermiteSpline(); }

        #region Gizmos

        public bool ShowTangents = true;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (Spline == null)
                return;

            if (ShowTangents)
            {
                using (new GizmosColorScope(Color.magenta))
                {
                    for (int i = 0; i < Spline.PointCount; ++i)
                    {
                        var p = Spline[i];
                        var t = Spline.Tangents[i];
                        GizmosEx.DrawLine(p, p + t, transform);
                    }
                }
            }
        }

        #endregion Gizmos
    }
}
