using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a curve that well interpolated from hermite spline.
    /// </summary>
    [Serializable]
    public class SplineCurve<T> : ISerializationCallbackReceiver
        where T : ISpline, new()
    {
        #region Spline

        public int KeyPointCount => _spline.PointCount;

        public IEnumerable<Vector3> KeyPoints => _spline;

        public void AddKeyPoint(Vector3 point)
        {
            _spline.AddPoint(point);
            if (_spline.PointCount > 1)
                _segments.Add(new Segment());

            MarkSegmentDirty(_segments.Count - 1);
            MarkSegmentDirty(_segments.Count - 2);
        }

        public void AddKeyPoints(IEnumerable<Vector3> points)
        {
            foreach (var point in points)
                AddKeyPoint(point);
        }

        public void InsertKeyPoint(int index, Vector3 point)
        {
            _spline.InsertPoint(index, point);

            if (_spline.PointCount > 1)
                _segments.Insert(index, new Segment());
            MarkSegmentDirty(index - 2);
            MarkSegmentDirty(index - 1);
            MarkSegmentDirty(index);
            MarkSegmentDirty(index + 1);
        }

        public void SetKeyPoint(int index, Vector3 point)
        {
            _spline[index] = point;

            MarkSegmentDirty(index - 2);
            MarkSegmentDirty(index - 1);
            MarkSegmentDirty(index);
            MarkSegmentDirty(index + 1);
        }

        public void RemoveKeyPoint(int index)
        {
            _spline.RemovePoint(index);

            if (_segments.Count > 0)
                _segments.RemoveAt(index);
            MarkSegmentDirty(index - 2);
            MarkSegmentDirty(index - 1);
            MarkSegmentDirty(index);
        }

        public void ClearKeyPoints()
        {
            _spline.ClearPoints();
            _segments.Clear();
        }

        public Vector3 GetKeyPoint(int index) { return _spline[index]; }

        private readonly T _spline = new T();

        #endregion Spline

        #region Segments

        /// <summary>
        ///     Get a enumerable collection which enumerates all points of current curve.
        /// </summary>
        /// <remarks>
        ///     You can also enumerates all points by enumerates <see cref="Segment.Points" /> of all <see cref="Segments" />,
        ///     but you have to take care of the duplication at head/tail of each segment. With this property the situation
        ///     never happened.
        /// </remarks>
        public IEnumerable<Vector3> Points
        {
            get
            {
                foreach (var segment in Segments)
                {
                    for (int i = 0; i < segment.Points.Count - 1; ++i)
                        yield return segment.Points[i];
                }

                if (TryGetSegment(SegmentCount - 1, out var lastSeg))
                {
                    var points = lastSeg.Points;
                    if (points.Count > 0)
                        yield return points[points.Count - 1];
                }
            }
        }

        /// <summary>
        ///     Represents the the segment between adjacent key points.
        /// </summary>
        public sealed class Segment
        {
            public IReadOnlyList<Vector3> Points => CurvePoints;

            internal bool Dirty;
            internal readonly List<Vector3> CurvePoints = new List<Vector3>();
        }

        public int SegmentCount => _segments.Count;

        public IEnumerable<Segment> Segments
        {
            get
            {
                for (int i = 0; i < _segments.Count; ++i)
                {
                    UpdateSegment(i);
                    yield return _segments[i];
                }
            }
        }

        public Segment GetSegment(int index)
        {
            Ensure.Argument.ValidIndex(index, _segments, nameof(index));
            UpdateSegment(index);
            return _segments[index];
        }

        public bool TryGetSegment(int index, out Segment segment)
        {
            segment = null;
            if (!Index.IsValid(index, _segments.Count))
                return false;

            UpdateSegment(index);
            segment = _segments[index];
            return true;
        }

        // Curve segments between each adjacent key points.
        private readonly List<Segment> _segments = new List<Segment>();

        private void MarkSegmentDirty(int index)
        {
            if (Index.IsValid(index, _segments.Count))
                _segments[index].Dirty = true;
        }

        private bool UpdateSegment(int index)
        {
            var seg = _segments[index];
            if (!seg.Dirty)
                return false;

            var points = seg.CurvePoints;
            points.Clear();

            // Add points aside of left key point of the segment.
            var pcl = GetPointCountAsideOfKeyPoint(index); // Point Count of Left side.
            for (int i = 0; i < pcl; ++i)
            {
                float t = GetInterpolationValue(i, pcl);
                points.Add(_spline.Interpolate(index, t));
            }

            // Add middle point of the segment.
            points.Add(_spline.Interpolate(index, 0.5f));

            // Add points aside of right key point of the segment.
            var pcr = GetPointCountAsideOfKeyPoint(index + 1); // Point Count of Right side.
            for (int i = pcr - 1; i >= 0; --i)
            {
                float t = 1 - GetInterpolationValue(i, pcr);
                points.Add(_spline.Interpolate(index, t));
            }

            seg.Dirty = false;
            return true;
        }

        private float GetInterpolationValue(int index, int count)
        {
            float t = (float)index / count;
            // x=0.629960524947437 -> y=0.5
            return Mathf.Pow(t * 0.629960524947437f, 1.5f);
        }

        private const int MaxPointsAsideOfKeyPoint = 10;

        private int GetPointCountAsideOfKeyPoint(int index)
        {
            var s0 = -_spline.GetSegment(index - 1);
            var s1 = _spline.GetSegment(index);
            if (s0.IsValid && s1.IsValid)
            {
                float angle = Mathf.Abs(Vector3.SignedAngle(s0.Vector, s1.Vector, Vector3.forward));
                // The lower angle, the more lines.
                return Mathf.CeilToInt((1 - angle / 180) * MaxPointsAsideOfKeyPoint) + 1;
            }
            return 0;
        }

        #endregion Segments

        #region Serialization

        [SerializeField] private Vector3[] _keyPoints;

        void ISerializationCallbackReceiver.OnBeforeSerialize() { _keyPoints = _spline.ToArray(); }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ClearKeyPoints();
            AddKeyPoints(_keyPoints);
        }

        #endregion Serialization
    }
}