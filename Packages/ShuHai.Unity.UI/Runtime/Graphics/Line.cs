using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.Unity.UI
{
    using IntervalF = Interval<float>;

    [AddComponentMenu("UI/Line")]
    public class Line : Graphic
    {
        public static Line Create(Canvas targetCanvas = null)
        {
            var obj = new GameObject(nameof(Line)) { layer = BuiltinLayers.UI };
            var line = obj.AddComponent<Line>();
            if (targetCanvas)
            {
                var transform = line.rectTransform;
                transform.SetParent(targetCanvas.transform, false);
                transform.anchorMin = Vector2.zero;
                transform.anchorMax = Vector2.one;
                transform.sizeDelta = Vector2.zero;
            }
            return line;
        }

        #region Width

        public const float MinWidth = float.Epsilon * 100;

        public float Width
        {
            get => _width;
            set
            {
                if (value == _width)
                    return;

                _width = value;
                ValidateWidth();

                SetVerticesDirty();
            }
        }

        [SerializeField] private float _width = 2;

        private void ValidateWidth()
        {
            if (_width < MinWidth)
                _width = MinWidth;
        }

        #endregion Width

        #region Points

        public int PointCount => _points.Count;

        public void AddPoint(Vector2 point)
        {
            _points.Add(point);
            SetVerticesDirty();
        }

        public void AddPoints(IEnumerable<Vector2> points)
        {
            foreach (var point in points)
                AddPoint(point);
        }

        public void RemovePoint(int index)
        {
            _points.RemoveAt(index);
            SetVerticesDirty();
        }

        public void SetPoint(int index, Vector2 point)
        {
            _points[index] = point;
            SetVerticesDirty();
        }

        public void SetPoints(IEnumerable<Vector2> points)
        {
            _points = new List<Vector2>(points);
            SetVerticesDirty();
        }

        public void ClearPoints()
        {
            _points.Clear();
            SetVerticesDirty();
        }

        public Vector2 GetPoint(int index) { return _points[index]; }

        [SerializeField] private List<Vector2> _points = new List<Vector2>();

        #endregion Points

        #region Mesh

        private struct Vertex
        {
            public Vector2 Position;
            public Vector2 UV;

            public Vertex(Vector2 position, Vector2 uv)
            {
                Position = position;
                UV = uv;
            }
        }

        private const int CornerSides = 5;
        private const float CornerAngleUnit = 180f / CornerSides;
        private const int CornerVertCount = CornerSides + 2;
        private const int LineVertCount = CornerVertCount * 2;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            int pointCount = _points.Count;
            if (pointCount < 2)
                return;

            var o = GetOriginForMesh();
            Color32 color32 = color;

            for (int i = 0, vertIdx0 = 0; i < pointCount - 1; ++i, vertIdx0 += LineVertCount)
            {
                Vector2 p0 = o + _points[i], p1 = o + _points[i + 1];
                if (p0.AlmostEquals(p1))
                    continue;

                var vertices0 = EvaluateSemiCircleVertices(p0, p0 - p1, true);
                var vertices1 = EvaluateSemiCircleVertices(p1, p1 - p0, false);
                foreach (var vertex in vertices0)
                    vh.AddVert(vertex.Position, color32, vertex.UV);
                foreach (var vertex in vertices1)
                    vh.AddVert(vertex.Position, color32, vertex.UV);

                // Add corner semi-circle triangles.
                for (int vi = 1; vi < CornerSides + 1; ++vi)
                {
                    int vi0 = vertIdx0, vi1 = CornerVertCount + vertIdx0;
                    vh.AddTriangle(vi0, vi0 + vi, vi0 + vi + 1);
                    vh.AddTriangle(vi1, vi1 + vi, vi1 + vi + 1);
                }

                // Add square triangles.
                int ci0 = vertIdx0 + 1, ci1 = ci0 + CornerSides;
                int ci2 = vertIdx0 + CornerVertCount + 1, ci3 = ci2 + CornerSides;
                vh.AddTriangle(ci0, ci1, ci2);
                vh.AddTriangle(ci2, ci3, ci0);
            }
        }

        private IEnumerable<Vertex> EvaluateSemiCircleVertices(Vector2 center, Vector2 lineVector, bool uvDir)
        {
            var lineLen = lineVector.magnitude;
            var fullLen = lineLen + 2 * _width;

            float coordVRange = _width / fullLen;
            float coordV0 = uvDir ? coordVRange : 1 - coordVRange;
            yield return new Vertex(center, new Vector2(0.5f, coordV0));

            var ccw90 = Quaternion.AngleAxis(90, Vector3.forward);
            var lineDir = lineLen > 0 ? lineVector / lineLen : Vector2.zero;
            Vector2 startPosVec = ccw90 * lineDir * _width;
            var startUVVec = Vector2.up * 0.5f;
            for (int s = 0; s <= CornerSides; ++s)
            {
                var cwUnit = Quaternion.AngleAxis(CornerAngleUnit * s, -Vector3.forward);
                Vector2 posVec = cwUnit * startPosVec, uvVec = cwUnit * startUVVec;
                var pos = center + posVec;
                uvVec.y = uvDir ? coordVRange * (1 - uvVec.y) : (1 - coordVRange) * uvVec.y;
                yield return new Vertex(pos, Vector2.zero);
            }
        }

        private Vector2 GetOriginForMesh() { return GetPixelAdjustedRect().min; }

        #endregion Mesh

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateWidth();
        }

        private void OnDrawGizmosSelected()
        {
            var o = GetOriginForMesh();
            using (new GizmosColorScope(Color.cyan))
            {
                for (int i = 0; i < _points.Count - 1; ++i)
                    GizmosEx.DrawLine(o + _points[i], o + _points[i + 1], transform);
                foreach (var point in _points)
                    GizmosEx.DrawRhombus(o + point, _width / 2, transform);
            }
        }
#endif
    }
}