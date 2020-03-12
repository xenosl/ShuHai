using System;
using UnityEngine;

namespace ShuHai.Unity
{
    public static class GizmosEx
    {
        public static void DrawLine(Vector3 from, Vector3 to, Transform transform)
        {
            if (transform != null)
            {
                from = transform.TransformPoint(from);
                to = transform.TransformPoint(to);
            }
            Gizmos.DrawLine(from, to);
        }

        public static void DrawCross(Vector2 position, float size = 0.25f, Transform transform = null)
        {
            float hs = size / 2;
            Vector2 l = position + Vector2.left * hs, r = position + Vector2.right * hs;
            Vector2 u = position + Vector2.up * hs, d = position + Vector2.down * hs;
            DrawLine(l, r, transform);
            DrawLine(u, d, transform);
        }

        public static void DrawRectangle(Rect rect, Transform transform = null)
        {
            DrawRectangle(rect.center, rect.size, transform);
        }

        public static void DrawRectangle(Vector2 center, Vector2 size, Transform transform = null)
        {
            var hs = size / 2;
            Vector2 bl = center - hs, tr = center + hs;
            var br = center + new Vector2(hs.x, -hs.y);
            var tl = center + new Vector2(-hs.x, hs.y);
            if (transform != null)
            {
                bl = transform.TransformPoint(bl);
                tr = transform.TransformPoint(tr);
                br = transform.TransformPoint(br);
                tl = transform.TransformPoint(tl);
            }

            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
        }

        public static void DrawBounds(Bounds bounds) { DrawBounds(bounds.center, bounds.extents); }

        public static void DrawBounds(Vector3 center, Vector3 extents)
        {
            var lbb = center - extents; // Left-Bottom-Back
            var rbb = new Vector3(center.x + extents.x, lbb.y, lbb.z); // Right-Bottom-Back
            var ltb = new Vector3(lbb.x, center.y + extents.y, lbb.z); // Left-Top-Back
            var lbf = new Vector3(lbb.x, lbb.y, center.z + extents.z); // Left-Bottom-Front
            var rtf = center + extents; // Right-Top-Front
            var ltf = new Vector3(center.x - extents.x, rtf.y, rtf.z); // Left-Top-Front
            var rbf = new Vector3(rtf.x, center.y - extents.y, rtf.z); // Right-Bottom-Front
            var rtb = new Vector3(rtf.x, rtf.y, center.z - extents.z); // Right-Top-Back

            Gizmos.DrawLine(lbb, rbb);
            Gizmos.DrawLine(lbb, ltb);
            Gizmos.DrawLine(lbb, lbf);
            Gizmos.DrawLine(rtf, ltf);
            Gizmos.DrawLine(rtf, rbf);
            Gizmos.DrawLine(rtf, rtb);
            Gizmos.DrawLine(rbb, rtb);
            Gizmos.DrawLine(rbb, rbf);
            Gizmos.DrawLine(ltb, rtb);
            Gizmos.DrawLine(ltb, ltf);
            Gizmos.DrawLine(lbf, rbf);
            Gizmos.DrawLine(lbf, ltf);
        }

        #region Circle 

        public static int DefaultCircleSides = 32;

        public static void DrawCircle(Vector3 position, Vector3 up, float radius)
        {
            DrawCircle(position, up, radius, DefaultCircleSides);
        }

        public static void DrawCircle(Vector3 position, Vector3 up, float radius, int sides)
        {
            var upr = up.normalized * radius;
            var forward = Vector3.Slerp(upr, -upr, 0.5f);
            var right = Vector3.Cross(upr, forward).normalized * radius;

            var matrix = new Matrix4x4
            {
                [0] = right.x,
                [1] = right.y,
                [2] = right.z,
                [4] = upr.x,
                [5] = upr.y,
                [6] = upr.z,
                [8] = forward.x,
                [9] = forward.y,
                [10] = forward.z
            };

            var lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));
            var nextPoint = Vector3.zero;

            float unitRad = 2 * Mathf.PI / sides;
            for (var i = 0; i <= sides; i++)
            {
                nextPoint.x = Mathf.Cos(i * unitRad);
                nextPoint.z = Mathf.Sin(i * unitRad);
                nextPoint.y = 0;

                nextPoint = position + matrix.MultiplyPoint3x4(nextPoint);

                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }

        public static void DrawCircle(Vector2 position, float radius, Transform transform = null)
        {
            DrawCircle(position, radius, DefaultCircleSides, transform);
        }

        public static void DrawCircle(Vector2 position, float radius, int sides, Transform transform)
        {
            if (transform != null)
            {
                position = transform.TransformPoint(position);
                var s = transform.lossyScale;
                radius *= Mathf.Max(s.x, s.y);
            }
            DrawCircle(position, Vector3.forward, radius, sides);
        }

        public static void DrawRhombus(Vector2 position, float size, Transform transform = null)
        {
            DrawCircle(position, size / 2, 4, transform);
        }

        #endregion Circle
    }

    public struct GizmosColorScope : IDisposable
    {
        public readonly Color PreservedColor;

        public GizmosColorScope(Color color)
        {
            PreservedColor = Gizmos.color;
            Gizmos.color = color;
        }

        public void Dispose() { Gizmos.color = PreservedColor; }
    }
}
