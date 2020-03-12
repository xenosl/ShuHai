using System;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Represents a 2-Dimensional segment.
    /// </summary>
    [Serializable]
    public struct Segment2
    {
        /// <summary>
        ///     First end point of current segment.
        /// </summary>
        public Vector2 P;

        /// <summary>
        ///     Second end point of current segment.
        /// </summary>
        public Vector2 Q;

        /// <summary>
        ///     Access end point of current segment by point index.
        /// </summary>
        /// <param name="index">Index of end point to access.</param>
        /// <exception cref="IndexOutOfRangeException">
        ///     <param name="index"> is greater than 1 or less than 0.</param>
        /// </exception>
        public Vector2 this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return P;
                    case 1: return Q;
                    default:
                        throw new IndexOutOfRangeException("Invalid Segment2 index.");
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
                        throw new IndexOutOfRangeException("Invalid Segment2 index.");
                }
            }
        }

        /// <summary>
        ///     Indicates whether the current instance is a valid segment (which length is not 0).
        /// </summary>
        public bool IsValid => Vector != Vector2.zero;

        /// <summary>
        ///     Vector from point <see cref="P" /> to point <see cref="Q" />.
        /// </summary>
        public Vector2 Vector => Q - P;

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
        public Segment2(Vector2 p, Vector2 q)
        {
            P = p;
            Q = q;
        }

        public Vector2 PointAt(float position) { return P + Vector * position; }

        #region Point Project

        public static bool PointProject(Segment2 segment, Vector2 point, out Vector2 result)
        {
            return PointProject(segment.P, segment.Q, point, out result);
        }

        public static bool PointProject(Vector2 segP, Vector2 segQ, Vector2 point, out Vector2 result)
        {
            var segVec = segQ - segP;
            var projector = point - segP;
            var projected = projector.Project(segVec);
            result = segP + projected;

            float minX = Mathf.Min(segP.x, segQ.x), maxX = Mathf.Max(segP.x, segQ.x);
            float minY = Mathf.Min(segP.y, segQ.y), maxY = Mathf.Max(segP.y, segQ.y);
            return result.x >= minX && result.x <= maxX && result.y >= minY && result.y <= maxY;
        }

        public bool PointProject(Vector2 point, out Vector2 result) { return PointProject(this, point, out result); }

        #endregion Point Project

        #region Relation

        public static bool IsPerpendicular(Vector2 firstP, Vector2 firstQ,
            Vector2 secondP, Vector2 secondQ, float epsilon = float.Epsilon)
        {
            Vector2 r = firstQ - firstP, s = secondQ - secondP;
            return Vector2.Dot(r, s).AlmostEquals(0, epsilon);
        }

        public static bool IsPerpendicular(
            Segment2 first, Segment2 second, float epsilon = float.Epsilon)
        {
            return IsPerpendicular(first.P, first.Q, second.P, second.Q, epsilon);
        }

        public static SegmentRelation Relation(
            Segment2 first, Segment2 second, float epsilon = float.Epsilon)
        {
            return Relation(first.P, first.Q, second.P, second.Q, epsilon);
        }

        public static SegmentRelation Relation(
            Vector2 firstP, Vector2 firstQ, Vector2 secondP, Vector2 secondQ,
            float epsilon = float.Epsilon)
        {
            return RelationInfo(firstP, firstQ, secondP, secondQ, out _, epsilon);
        }

        public static SegmentRelation RelationInfo(
            Segment2 first, Segment2 second, out Vector2 intersection, float epsilon = float.Epsilon)
        {
            return RelationInfo(first.P, first.Q, second.P, second.Q, out intersection, epsilon);
        }

        /// <summary>
        ///     Found it at:
        ///     http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
        ///     Algorithm from Graphics Gems, page 304.
        /// </summary>
        public static SegmentRelation RelationInfo(
            Vector2 firstP, Vector2 firstQ, Vector2 secondP, Vector2 secondQ,
            out Vector2 intersection, float epsilon = float.Epsilon)
        {
            Vector2 r = firstQ - firstP, s = secondQ - secondP;
            var w = secondP - firstP;
            float rXs = r.Cross(s);
            float wXr = w.Cross(r);

            if (rXs.AlmostEquals(0, epsilon))
            {
                if (wXr.AlmostEquals(0, epsilon))
                {
                    float rds = Vector2.Dot(r, s);
                    float sqrr = r.sqrMagnitude;
                    float t0 = Vector2.Dot(w, r) / sqrr;
                    float t1 = t0 + rds / sqrr;
                    if (t0 < 0 && t1 > 1)
                    {
                        intersection = firstP;
                        return SegmentRelation.CollinearOverlap;
                    }
                    else if (t0 >= 0 && t0 <= 1)
                    {
                        intersection = firstP + t0 * r;
                        return SegmentRelation.CollinearOverlap;
                    }
                    else if (t1 >= 0 && t1 <= 1)
                    {
                        intersection = firstP + t1 * r;
                        return SegmentRelation.CollinearOverlap;
                    }
                    else
                    {
                        intersection = firstP + t0 * r;
                        return SegmentRelation.CollinearDisjoint;
                    }
                }
                else
                {
                    intersection = Vectors.NaN2;
                    return SegmentRelation.Parallel;
                }
            }
            else
            {
                float t = w.Cross(s) / rXs;
                float u = wXr / rXs;
                intersection = firstP + t * r;
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                    return SegmentRelation.Intersect;
                else
                    return SegmentRelation.Disjoint;
            }
        }

        #endregion Relation

        #region Closet Point

        public static Vector2 ClosestPoint(Vector2 segP, Vector2 segQ, Vector2 toMe)
        {
            return ClosestPoint(new Segment2(segP, segQ), toMe);
        }

        public static Vector2 ClosestPoint(Segment2 segment, Vector2 toMe)
        {
            Vector2 pointOnSegLine;
            PointProject(segment, toMe, out pointOnSegLine);
            var pointVec = pointOnSegLine - segment.P;
            float s = Vector2.Dot(pointVec, segment.Vector) / segment.SqrLength;
            if (s <= 0)
                return segment.P;
            if (s >= 1)
                return segment.Q;
            return pointOnSegLine;
        }

        public static void ClosestPoints(Segment2 first, Segment2 second,
            out Vector2 pointOnFirst, out Vector2 pointOnSecond, float epsilon = float.Epsilon)
        {
            ClosestPoints(first.P, first.Q, second.P, second.Q,
                out pointOnFirst, out pointOnSecond, epsilon);
        }

        /// <summary>
        ///     Algorithm from: http://geomalgorithms.com/a07-_distance.html
        /// </summary>
        public static void ClosestPoints(
            Vector2 firstP, Vector2 firstQ, Vector2 secondP, Vector2 secondQ,
            out Vector2 pointOnFirst, out Vector2 pointOnSecond, float epsilon = float.Epsilon)
        {
            var u = firstQ - firstP;
            var v = secondQ - secondP;
            var w = firstP - secondP;
            float a = Vector2.Dot(u, u); // always >= 0
            float b = Vector2.Dot(u, v);
            float c = Vector2.Dot(v, v); // always >= 0
            float d = Vector2.Dot(u, w);
            float e = Vector2.Dot(v, w);
            float D = a * c - b * b; // always >= 0
            float sc, sN, sD = D; // sc = sN / sD, default sD = D >= 0
            float tc, tN, tD = D; // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < epsilon) // the lines are almost parallel
            {
                sN = 0; // force using point P0 on segment S1
                sD = 1; // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else // get the closest points on the infinite lines
            {
                sN = b * e - c * d;
                tN = a * e - b * d;
                if (sN < 0) // sc < 0 => the s=0 edge is visible
                {
                    sN = 0;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD) // sc > 1  => the s=1 edge is visible
                {
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0) // tc < 0 => the t=0 edge is visible
            {
                tN = 0;
                // recompute sc for this edge
                if (-d < 0)
                {
                    sN = 0;
                }
                else if (-d > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD) // tc > 1  => the t=1 edge is visible
            {
                tN = tD;
                // recompute sc for this edge
                if (-d + b < 0)
                {
                    sN = 0;
                }
                else if (-d + b > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d + b;
                    sD = a;
                }
            }

            // finally do the division to get sc and tc
            sc = Mathf.Abs(sN) < epsilon ? 0 : sN / sD;
            tc = Mathf.Abs(tN) < epsilon ? 0 : tN / tD;
            pointOnFirst = firstP + sc * u;
            pointOnSecond = secondP + tc * v;
        }

        public Vector2 ClosestPoint(Vector2 toMe) { return ClosestPoint(this, toMe); }

        #endregion Closet Point

        #region Distance

        public static float Distance(Segment2 first, Segment2 second, float epsilon = float.Epsilon)
        {
            return Distance(first.P, first.Q, second.P, second.Q, epsilon);
        }

        public static float Distance(Vector2 firstP, Vector2 firstQ,
            Vector2 secondP, Vector2 secondQ, float epsilon = float.Epsilon)
        {
            ClosestPoints(firstP, firstQ, secondP, secondQ, out var p1, out var p2, epsilon);
            return (p1 - p2).magnitude;
        }

        public static float SqrDistance(Segment2 first, Segment2 second, float epsilon = float.Epsilon)
        {
            return SqrDistance(first.P, first.Q, second.P, second.Q, epsilon);
        }

        public static float SqrDistance(Vector2 firstP, Vector2 firstQ,
            Vector2 secondP, Vector2 secondQ, float epsilon = float.Epsilon)
        {
            Vector2 p1, p2;
            ClosestPoints(firstP, firstQ, secondP, secondQ, out p1, out p2, epsilon);
            return (p1 - p2).sqrMagnitude;
        }

        public static float Distance(Segment2 segment, Vector2 point) { return Distance(segment.P, segment.Q, point); }

        public static float Distance(Vector2 p, Vector2 q, Vector2 point)
        {
            return Mathf.Sqrt(SqrDistance(p, q, point));
        }

        public static float SqrDistance(Segment2 segment, Vector2 point)
        {
            return SqrDistance(segment.P, segment.Q, point);
        }

        public static float SqrDistance(Vector2 p, Vector2 q, Vector2 point)
        {
            var cp = ClosestPoint(p, q, point);
            return (cp - point).sqrMagnitude;
        }

        public static float DistanceAsLine(Segment2 segment, Vector2 point)
        {
            return Mathf.Sqrt(SqrDistanceAsLine(segment.P, segment.Q, point));
        }

        public static float DistanceAsLine(Vector2 p, Vector2 q, Vector2 point)
        {
            return Mathf.Sqrt(SqrDistanceAsLine(p, q, point));
        }

        public static float SqrDistanceAsLine(Segment2 segment, Vector2 point)
        {
            return SqrDistanceAsLine(segment.P, segment.Q, point);
        }

        public static float SqrDistanceAsLine(Vector2 p, Vector2 q, Vector2 point)
        {
            var segVec = q - p;
            var projector = point - p;
            var projected = projector.Project(segVec);
            return (p + projected - point).sqrMagnitude;
        }

        public float Distance(Vector2 point) { return Distance(P, Q, point); }

        public float SqrDistance(Vector2 point) { return SqrDistance(P, Q, point); }

        public float DistanceAsLine(Vector2 point) { return DistanceAsLine(P, Q, point); }

        public float SqrDistanceAsLine(Vector2 point) { return SqrDistanceAsLine(P, Q, point); }

        #endregion Distance

        #region Intersect

        public static bool Intersect(
            Segment2 first, Segment2 second,
            out Vector2 point, float epsilon = float.Epsilon)
        {
            return Intersect(first.P, first.Q, second.P, second.Q, out point, epsilon);
        }

        public static bool Intersect(
            Vector2 firstP, Vector2 firstQ, Vector2 secondP, Vector2 secondQ,
            out Vector2 point, float epsilon = float.Epsilon)
        {
            var r = RelationInfo(firstP, firstQ, secondP, secondQ, out point, epsilon);
            return r == SegmentRelation.CollinearOverlap
                || r == SegmentRelation.Intersect;
        }

        public static bool IntersectAsLine(
            Segment2 first, Segment2 second,
            out Vector2 point, float epsilon = float.Epsilon)
        {
            return IntersectAsLine(first.P, first.Q, second.P, second.Q, out point, epsilon);
        }

        public static bool IntersectAsLine(
            Vector2 firstP, Vector2 firstQ, Vector2 secondP, Vector2 secondQ,
            out Vector2 point, float epsilon = float.Epsilon)
        {
            var r = RelationInfo(firstP, firstQ, secondP, secondQ, out point, epsilon);
            return r == SegmentRelation.Intersect
                || r == SegmentRelation.Disjoint;
        }

        public bool Intersect(Segment2 other, out Vector2 point) { return Intersect(this, other, out point); }

        public bool Intersect(Vector2 p, Vector2 q, out Vector2 point) { return Intersect(P, Q, p, q, out point); }

        public bool IntersectAsLine(Segment2 other, out Vector2 point)
        {
            return IntersectAsLine(this, other, out point);
        }

        public bool IntersectAsLine(Vector2 p, Vector2 q, out Vector2 point)
        {
            return IntersectAsLine(P, Q, p, q, out point);
        }

        #region Circle

        public static int CircleIntersect(Segment2 segment, Vector2 center, float radius,
            out Vector2 intersection1, out Vector2 intersection2, float epsilon = float.Epsilon)
        {
            return CircleIntersect(segment.P, segment.Q,
                center, radius, out intersection1, out intersection2, epsilon);
        }

        public static int CircleIntersect(
            Vector2 segP, Vector2 segQ, Vector2 center, float radius,
            out Vector2 intersection1, out Vector2 intersection2, float epsilon = float.Epsilon)
        {
            intersection1 = Vectors.NaN2;
            intersection2 = Vectors.NaN2;

            float dx = segQ.x - segP.x, dy = segQ.y - segP.y;
            float A = dx * dx + dy * dy;
            float B = 2 * (dx * (segP.x - center.x) + dy * (segP.y - center.y));
            float C = (segP.x - center.x) * (segP.x - center.x)
                + (segP.y - center.y) * (segP.y - center.y) - radius * radius;

            float det = B * B - 4 * A * C;
            if (A <= epsilon || det < 0)
            {
                // No real solutions.
                return 0;
            }
            else if (det == 0)
            {
                // One solution.
                float t = -B / (2 * A);
                if (t >= 0 && t <= 1)
                {
                    intersection1 = new Vector2(segP.x + t * dx, segP.y + t * dy);
                    return 1;
                }

                return 0;
            }
            else
            {
                // Two solutions.
                float t1 = (-B + Mathf.Sqrt(det)) / (2 * A);
                var intersectCount = 0;
                if (t1 >= 0 && t1 <= 1)
                {
                    intersection1 = new Vector2(segP.x + t1 * dx, segP.y + t1 * dy);
                    intersectCount++;
                }

                float t2 = (-B - Mathf.Sqrt(det)) / (2 * A);
                if (t2 >= 0 && t2 <= 1)
                {
                    if (intersectCount == 0)
                        intersection1 = new Vector2(segP.x + t2 * dx, segP.y + t2 * dy);
                    else
                        intersection2 = new Vector2(segP.x + t2 * dx, segP.y + t2 * dy);
                    intersectCount++;
                }

                return intersectCount;
            }
        }

        #endregion Circle

        #endregion Intersect

        #region Translate

        public void Translate(Vector2 t)
        {
            P += t;
            Q += t;
        }

        public Segment2 Translated(Vector2 t) { return new Segment2(P + t, Q + t); }

        #endregion Translate

        #region To String

        public override string ToString() { return $"({P}->{Q})"; }

        public string ToString(string format) { return $"({P.ToString(format)}->{Q.ToString(format)})"; }

        #endregion To String

        #region Equality

        public override bool Equals(object obj) { return obj is Segment2 other && Equals(other); }

        public bool Equals(Segment2 other) { return P == other.P && Q == other.Q; }

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

        public static bool operator ==(Segment2 l, Segment2 r) { return l.Equals(r); }
        public static bool operator !=(Segment2 l, Segment2 r) { return !(l == r); }

        #endregion Equality

        public static Segment2 operator +(Segment2 s) { return s; }
        public static Segment2 operator -(Segment2 s) { return new Segment2(s.Q, s.P); }

#if UNITY_EDITOR
        public void DrawGizmo() { Gizmos.DrawLine(P, Q); }
#endif
    }
}