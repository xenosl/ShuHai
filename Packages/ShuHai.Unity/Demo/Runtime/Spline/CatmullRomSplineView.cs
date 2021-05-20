using UnityEngine;

namespace ShuHai.Unity.Demo
{
    public class CatmullRomSplineView : SplineView
    {
        protected new CatmullRomSpline Spline => (CatmullRomSpline)base.Spline;

        protected override ISpline CreateSpline() { return new CatmullRomSpline(); }

        #region Gizmos

        [Range(0, 1)]
        public float Alpha = 0.5f;

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (Spline != null)
                Spline.Tension = Alpha;
        }

        #endregion Gizmos
    }
}
