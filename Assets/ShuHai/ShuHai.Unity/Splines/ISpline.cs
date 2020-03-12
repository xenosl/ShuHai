using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity
{
    public interface ISpline : IEnumerable<Vector3>
    {
        int PointCount { get; }

        Vector3 this[int index] { get; set; }

        void AddPoint(Vector3 point);
        void InsertPoint(int index, Vector3 point);
        void RemovePoint(int index);
        void ClearPoints();

        Vector3 Interpolate(int segmentIndex, float t);
    }
}