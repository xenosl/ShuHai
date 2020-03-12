using System;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a 3-Dimensional segment.
    /// </summary>
    [Serializable]
    public struct Segment3
    {
        /// <summary>
        ///     First end point of current segment.
        /// </summary>
        public Vector3 P;

        /// <summary>
        ///     Second end point of current segment.
        /// </summary>
        public Vector3 Q;

        /// <summary>
        ///     Access end point of current segment by point index.
        /// </summary>
        /// <param name="index">Index of end point to access.</param>
        /// <exception cref="IndexOutOfRangeException">
        ///     <param name="index"> is greater than 1 or less than 0.</param>
        /// </exception>
        public Vector3 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return P;
                    case 1: return Q;
                    default:
                        throw new IndexOutOfRangeException("Invalid Segment3 index.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        P = value;
                        break;
                    case 1:
                        Q = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Segment3 index.");
                }
            }
        }

        /// <summary>
        ///     Indicates whether the current instance is a valid segment (which length is not 0).
        /// </summary>
        public bool IsValid => Vector != Vector3.zero;

        /// <summary>
        ///     Vector from point <see cref="P" /> to point <see cref="Q" />.
        /// </summary>
        public Vector3 Vector => Q - P;

        /// <summary>
        ///     Squared length of current segment.
        /// </summary>
        public float SqrLength => Vector.sqrMagnitude;

        /// <summary>
        ///     Length of current segment.
        /// </summary>
        public float Length => Vector.magnitude;

        /// <summary>
        ///     Initialize a new instance of a 2-Dimensional segment with specified points.
        /// </summary>
        /// <param name="p">First end point of the segment.</param>
        /// <param name="q">Second end point of the segment.</param>
        public Segment3(Vector3 p, Vector3 q)
        {
            P = p;
            Q = q;
        }

        public Vector3 PointAt(float position) { return P + Vector * position; }

        #region Translate

        public void Translate(Vector3 t)
        {
            P += t;
            Q += t;
        }

        public Segment3 Translated(Vector3 t) { return new Segment3(P + t, Q + t); }

        #endregion Translate

        #region To String

        public override string ToString() { return $"({P}->{Q})"; }

        public string ToString(string format) { return $"({P.ToString(format)}->{Q.ToString(format)})"; }

        #endregion To String

        #region Equality

        public override bool Equals(object obj) { return obj is Segment3 other && Equals(other); }

        public bool Equals(Segment3 other) { return P == other.P && Q == other.Q; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + P.GetHashCode();
                hash = hash * 23 + Q.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Segment3 l, Segment3 r) { return l.Equals(r); }
        public static bool operator !=(Segment3 l, Segment3 r) { return !(l == r); }

        #endregion Equality

        public static Segment3 operator +(Segment3 s) { return s; }
        public static Segment3 operator -(Segment3 s) { return new Segment3(s.Q, s.P); }

#if UNITY_EDITOR
        public void DrawGizmo() { Gizmos.DrawLine(P, Q); }
#endif
    }
}