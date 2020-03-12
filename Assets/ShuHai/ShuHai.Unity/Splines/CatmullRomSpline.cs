using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a Catmull-Rom spline.
    /// </summary>
    [Serializable]
    public class CatmullRomSpline : ISpline, ISerializationCallbackReceiver
    {
        public int PointCount => _points.Count;

        public Vector3 this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        /// <summary>
        ///     Determines how 'tight' the current spline is, ranges from 0 to 1.
        ///     To make the spline smoother, greater value is preferred; otherwise lower value is better.
        ///     A value of 0 make the current spline not smooth at all, namely a collection of line segments.
        ///     The value is 0.5 by default.
        /// </summary>
        public float Tension
        {
            get => _tension;
            set
            {
                if (value == _tension)
                    return;
                _tension = Mathf.Clamp(value, 0, 1);
                UpdateCoefficient();
            }
        }

        public CatmullRomSpline() : this(Array.Empty<Vector3>()) { }

        public CatmullRomSpline(IEnumerable<Vector3> points)
        {
            AddPoints(points);
            UpdateCoefficient();
        }

        public void AddPoint(Vector3 point) { _points.Add(point); }

        public void AddPoints(IEnumerable<Vector3> points)
        {
            foreach (var point in points)
                AddPoint(point);
        }

        public void InsertPoint(int index, Vector3 point) { _points.Insert(index, point); }

        public void RemovePoint(int index) { _points.RemoveAt(index); }

        public void ClearPoints() { _points.Clear(); }

        public Vector3 Interpolate(int segmentIndex, float t)
        {
            int pc = PointCount;
            Ensure.Argument.ValidIndex(segmentIndex, pc, nameof(segmentIndex));

            if (segmentIndex == pc - 1) // Duff request, cannot blend to nothing, Just return source
                return _points[segmentIndex];

            Vector3 p1 = _points.Get(segmentIndex), p2 = _points.Get(segmentIndex + 1);
            if (t == 0.0f)
                return p1;
            if (t == 1.0f)
                return p2;

            return this.Interpolate(_coefficient, _points.Get(segmentIndex - 1), p1, p2, _points.Get(segmentIndex + 2), t);
        }

        public IEnumerator<Vector3> GetEnumerator() { return _points.GetEnumerator(); }

        private readonly SplinePoints _points = new SplinePoints();

        private float _tension = 0.5f;

        #region Coefficient

        private Matrix4x4 _coefficient;

        private void UpdateCoefficient()
        {
            _coefficient = new Matrix4x4
            {
                [0, 0] = -_tension, [0, 1] = 2 - _tension, [0, 2] = _tension - 2, [0, 3] = _tension,
                [1, 0] = 2 * _tension, [1, 1] = _tension - 3, [1, 2] = 3 - 2 * _tension, [1, 3] = -_tension,
                [2, 0] = -_tension, [2, 1] = 0, [2, 2] = _tension, [2, 3] = 0,
                [3, 0] = 0, [3, 1] = 1, [3, 2] = 0, [3, 3] = 0
            };
        }

        #endregion Coefficient

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