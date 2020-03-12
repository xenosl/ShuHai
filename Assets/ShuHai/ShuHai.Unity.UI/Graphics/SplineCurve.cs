using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.Unity.UI
{
    public class SplineCurve<T> : Graphic, ISerializationCallbackReceiver
        where T : ISpline, new()
    {
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

        #region Curve

        public int KeyPointCount => _curve.KeyPointCount;

        public IEnumerable<Vector2> KeyPoints => _curve.KeyPoints.Select(p => (Vector2)p);

        public void AddKeyPoint(Vector2 point)
        {
            _curve.AddKeyPoint(point);
            SetVerticesDirty();
        }

        public void AddKeyPoints(IEnumerable<Vector2> points)
        {
            foreach (var point in points)
                AddKeyPoint(point);
        }

        public void InsertKeyPoint(int index, Vector2 point)
        {
            _curve.InsertKeyPoint(index, point);
            SetVerticesDirty();
        }

        public void SetKeyPoint(int index, Vector2 point)
        {
            _curve.SetKeyPoint(index, point);
            SetVerticesDirty();
        }

        public void RemoveKeyPoint(int index)
        {
            _curve.RemoveKeyPoint(index);
            SetVerticesDirty();
        }

        public void ClearKeyPoints()
        {
            _curve.ClearKeyPoints();
            SetVerticesDirty();
        }

        public Vector2 GetKeyPoint(int index) { return _curve.GetKeyPoint(index); }

        private readonly ShuHai.Unity.SplineCurve<T> _curve = new ShuHai.Unity.SplineCurve<T>();

        #endregion Curve

        #region Mesh Generation

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
        private const int VertCountPerLinePiece = CornerVertCount * 2;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            int startVertIndex = 0;
            foreach (var segment in _curve.Segments)
            {
                PopulateSegment(vh, segment, startVertIndex);
                int lineCount = segment.Points.Count - 1;
                if (lineCount > 0)
                    startVertIndex += lineCount * VertCountPerLinePiece;
            }
        }

        private void PopulateSegment(VertexHelper vh, ShuHai.Unity.SplineCurve<T>.Segment segment, int startVertIndex)
        {
            var points = segment.Points;
            int pointCount = points.Count;
            if (pointCount < 2)
                return;

            var o = GetOriginForMesh();
            Color32 color32 = color;

            for (int i = 0, vertIdx0 = startVertIndex; i < pointCount - 1; ++i, vertIdx0 += VertCountPerLinePiece)
            {
                Vector2 p0 = o + (Vector2)points[i], p1 = o + (Vector2)points[i + 1];
                if (p0.AlmostEquals(p1))
                    continue;

                // Add vertices.
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

        #endregion Mesh Generation

        #region Serialization

        [SerializeField] private Vector2[] _keyPoints;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keyPoints = _curve.KeyPoints.Select(p => (Vector2)p).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _curve.ClearKeyPoints();
            if (!CollectionUtil.IsNullOrEmpty(_keyPoints))
                _curve.AddKeyPoints(_keyPoints.Select(p => (Vector3)p));
        }

        #endregion Serialization

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
                foreach (var segment in _curve.Segments)
                {
                    var points = segment.Points;
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        Vector2 p0 = points[i], p1 = points[i + 1];
                        GizmosEx.DrawLine(o + p0, o + p1, transform);
                    }

                    foreach (var point in points)
                        GizmosEx.DrawCross(o + (Vector2)point, _width / 2, transform);
                }
            }

            using (new GizmosColorScope(Color.magenta))
            {
                foreach (var point in _curve.KeyPoints)
                    GizmosEx.DrawCross(o + (Vector2)point, _width / 2, transform);
            }
        }
#endif
    }
}