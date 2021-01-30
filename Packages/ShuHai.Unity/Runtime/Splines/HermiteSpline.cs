using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Collections;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a Hermite spline.
    /// </summary>
    [Serializable]
    public class HermiteSpline : ISpline, ISerializationCallbackReceiver
    {
        public int PointCount => _points.Count;

        public Vector3 this[int index]
        {
            get => _points[index];
            set
            {
                if (value == _points[index])
                    return;

                _points[index] = value;

                UpdateTangent(index - 1);
                UpdateTangent(index);
                UpdateTangent(index + 1);
            }
        }

        /// <summary>
        ///     Make the current spline a closed spline.
        /// </summary>
        public bool Closed
        {
            get => _closed;
            set
            {
                if (value == _closed)
                    return;
                _closed = value;
                UpdateTangent(0);
                UpdateTangent(_points.Count - 1);
            }
        }

        /// <summary>
        ///     A list of tangent vectors of each control point.
        /// </summary>
        public IReadOnlyList<Vector3> Tangents => _tangents;

        public HermiteSpline() : this(Array.Empty<Vector3>()) { }

        public HermiteSpline(IEnumerable<Vector3> points) { AddPoints(points); }

        public void AddPoint(Vector3 point)
        {
            _points.AddLast(point);

            _tangents.AddLast(Vector3.zero);
            var pc = _points.Count;
            UpdateTangent(pc - 2);
            UpdateTangent(pc - 1);
        }

        public void AddPoints(IEnumerable<Vector3> points)
        {
            foreach (var point in points)
                AddPoint(point);
        }

        public void InsertPoint(int index, Vector3 point)
        {
            Ensure.Argument.ValidIndex(index, _points, nameof(index));

            _points.Insert(index, point);

            _tangents.Insert(index, Vector3.zero);
            UpdateTangent(index - 1);
            UpdateTangent(index);
            UpdateTangent(index + 1);
        }

        public void InsertPoints(int index, IEnumerable<Vector3> points)
        {
            Ensure.Argument.ValidIndex(index, _points, nameof(index));

            foreach (var point in points)
                InsertPoint(index++, point);
        }

        public void RemovePoint(int index)
        {
            Ensure.Argument.ValidIndex(index, _points, nameof(index));

            _points.RemoveAt(index);

            _tangents.RemoveAt(index);
            UpdateTangent(index - 1);
            UpdateTangent(index);
        }

        public void ClearPoints()
        {
            _points.Clear();
            _tangents.Clear();
        }

        /// <summary>
        ///     Interpolates between the point at the specified index and the point at the specified index+1 by the specified
        ///     parametric value.
        /// </summary>
        /// <param name="segmentIndex">Index of the first point for the interpolation.</param>
        /// <param name="t">
        ///     Parametric value ranges from 0 to 1 for the interpolation that represents the position between the point
        ///     at <paramref name="segmentIndex" /> and the point at <paramref name="segmentIndex" />+1.
        /// </param>
        public Vector3 Interpolate(int segmentIndex, float t)
        {
            int pc = PointCount;
            Ensure.Argument.ValidIndex(segmentIndex, pc, nameof(segmentIndex));

            if (segmentIndex == pc - 1) // Duff request, cannot blend to nothing, Just return source
                return _points[segmentIndex];

            Vector3 p1 = _points[segmentIndex], p2 = _points[segmentIndex + 1];
            if (t == 0.0f)
                return p1;
            if (t == 1.0f)
                return p2;

            return this.Interpolate(Coefficient, p1, p2, _tangents[segmentIndex], _tangents[segmentIndex + 1], t);
        }

        public IEnumerator<Vector3> GetEnumerator() { return _points.GetEnumerator(); }

        private readonly Deque<Vector3> _points = new Deque<Vector3>();
        private readonly Deque<Vector3> _tangents = new Deque<Vector3>();

        private bool _closed;

        private void UpdateTangent(int index)
        {
            int count = _tangents.Count;
            Debug.Assert(_points.Count == count);

            if (count < 2)
                return;

            if (count == 2)
            {
                if (IsValidIndex(index))
                    _tangents[index] = _points[1] - _points[0];
                return;
            }

            if (_closed)
                index = Index.Loop(index, count);

            if (!IsValidIndex(index))
                return;

            Vector3 prev, next;
            if (_closed)
            {
                prev = _points[Index.Loop(index - 1, count)];
                next = _points[Index.Loop(index + 1, count)];
            }
            else
            {
                if (index == 0)
                {
                    prev = _points[0];
                    next = _points[1];
                }
                else if (index == count - 1)
                {
                    prev = _points[count - 2];
                    next = _points[count - 1];
                }
                else
                {
                    prev = _points[index - 1];
                    next = _points[index + 1];
                }
            }
            _tangents[index] = (next - prev) * 0.5f;
        }

        private bool IsValidIndex(int index) { return Index.IsValid(index, _points.Count); }

        private static readonly Matrix4x4 Coefficient = new Matrix4x4
        {
            [0, 0] = 2, [0, 1] = -2, [0, 2] = 1, [0, 3] = 1,
            [1, 0] = -3, [1, 1] = 3, [1, 2] = -2, [1, 3] = -1,
            [2, 0] = 0, [2, 1] = 0, [2, 2] = 1, [2, 3] = 0,
            [3, 0] = 1, [3, 1] = 0, [3, 2] = 0, [3, 3] = 0
        };

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #region Serialization

        [SerializeField] private Vector3[] _keyPoints;

        void ISerializationCallbackReceiver.OnBeforeSerialize() { _keyPoints = _points.ToArray(); }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ClearPoints();
            AddPoints(_keyPoints);
        }

        #endregion Serialization
    }
}