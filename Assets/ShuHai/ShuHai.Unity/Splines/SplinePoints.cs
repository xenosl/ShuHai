using System;
using System.Collections;
using System.Collections.Generic;
using ShuHai.Collections;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents spline point list with extra head and tail points.
    /// </summary>
    internal class SplinePoints : IList<Vector3>
    {
        public int Count => _points.Count;

        public Vector3 this[int index]
        {
            get => _points[index];
            set
            {
                _points[index] = value;

                if (index == 0)
                    UpdateHeadPoint();
                else if (index == Count - 1)
                    UpdateTailPoint();
            }
        }

        public void Add(Vector3 point)
        {
            _points.AddLast(point);
            UpdateEndPoints();
        }

        public int IndexOf(Vector3 item) { throw new NotImplementedException(); }

        public void Insert(int index, Vector3 point)
        {
            _points.Insert(index, point);
            UpdateEndPoints();
        }

        public void CopyTo(Vector3[] array, int arrayIndex) { throw new NotImplementedException(); }

        public bool Remove(Vector3 point)
        {
            bool removed = _points.Remove(point);
            if (removed)
                UpdateEndPoints();
            return removed;
        }

        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
            UpdateEndPoints();
        }

        public void Clear()
        {
            _points.Clear();
            UpdateEndPoints();
        }

        public bool Contains(Vector3 item) { throw new NotImplementedException(); }

        public Vector3 Get(int index, bool withExtraPoints = true)
        {
            if (!withExtraPoints)
                return _points[index];

            if (Index.IsValid(index, Count))
                return _points[index];

            if (index == -1)
                return HeadPoint;
            if (index == Count)
                return TailPoint;

            throw new IndexOutOfRangeException("Index of spline point out of range.");
        }

        public IEnumerator<Vector3> GetEnumerator() { return _points.GetEnumerator(); }

        // Visible points for user.
        private readonly Deque<Vector3> _points = new Deque<Vector3>();

        #region End Points

        public Vector3 HeadPoint { get; private set; }
        public Vector3 TailPoint { get; private set; }

        private void UpdateEndPoints()
        {
            UpdateHeadPoint();
            UpdateTailPoint();
        }

        private void UpdateHeadPoint()
        {
            HeadPoint = Count >= 2 ? GetExtendPointOnLine(_points[1], _points[0]) : Vector3.zero;
        }

        private void UpdateTailPoint()
        {
            TailPoint = Count >= 2 ? GetExtendPointOnLine(_points[Count - 2], _points[Count - 1]) : Vector3.zero;
        }

        private static Vector3 GetExtendPointOnLine(Vector3 from, Vector3 to) { return to + (to - from); }

        #endregion End Points

        #region Explicit Implementations

        bool ICollection<Vector3>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion Explicit Implementations
    }
}