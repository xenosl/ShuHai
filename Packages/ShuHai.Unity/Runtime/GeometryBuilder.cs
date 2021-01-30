using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity
{
    public static class GeometryBuilder
    {
        public struct Vertex
        {
            public Vector3 Position;
            public Vector2 Texcoord;

            public Vertex(Vector3 position, Vector2 texcoord)
            {
                Position = position;
                Texcoord = texcoord;
            }
        }

        public struct Triangle
        {
            public int Index0, Index1, Index2;

            public Triangle(int index0, int index1, int index2)
            {
                Index0 = index0;
                Index1 = index1;
                Index2 = index2;
            }

            public int[] ToArray() { return new[] { Index0, Index1, Index2 }; }
        }

        #region Rectangle

        public static (Vertex[] vertices, Triangle[] triangles) BuildRectangle(
            Vector2 size, AxisPlane plane, bool clockwise = true)
        {
            var vertices = new Vertex[4];
            var triangles = new Triangle[2];
            BuildRectangle(vertices, triangles, size, plane, clockwise);
            return (vertices, triangles);
        }

        public static void BuildRectangle(
            IList<Vertex> vertices, IList<Triangle> triangles,
            Vector2 size, AxisPlane plane, bool clockwise = true)
        {
            BuildRectangle(vertices, 0, triangles, 0, size, plane, clockwise);
        }

        public static void BuildRectangle(
            IList<Vertex> vertices, int vertexStartIndex, IList<Triangle> triangles, int triangleStartIndex,
            Vector2 size, AxisPlane plane, bool clockwise = true)
        {
            Ensure.Argument.NotNull(vertices, nameof(vertices));
            if (vertices.Count < vertexStartIndex + 4)
                throw new ArgumentException("Insufficient size for vertices.", nameof(vertices));

            Ensure.Argument.NotNull(triangles, nameof(triangles));
            if (triangles.Count < triangleStartIndex + 2)
                throw new ArgumentException("Insufficient size for triangles.", nameof(vertices));

            BuildRectangle((index, vertex) => { vertices[vertexStartIndex + index] = vertex; },
                (index, triangle) => { triangles[triangleStartIndex + index] = triangle; },
                size, plane, clockwise);
        }

        public static void BuildRectangle(
            Action<int, Vertex> vertexReceiver, Action<int, Triangle> triangleReceiver,
            Vector2 size, AxisPlane plane, bool clockwise = true)
        {
            Ensure.Argument.NotNull(vertexReceiver, nameof(vertexReceiver));
            Ensure.Argument.NotNull(triangleReceiver, nameof(triangleReceiver));

            int normalAxis = (int)plane.GetNormalAxis();
            var hs = size / 2;
            for (int i = 0; i < 4; ++i)
            {
                float v1 = i < 2 ? -1 : 1, v2 = Index.Loop(i + 1, 4) < 2 ? -1 : 1;
                var pos = new Vector3
                {
                    [normalAxis] = 0,
                    [Index.Loop(normalAxis + 1, 3)] = v1 * hs.x,
                    [Index.Loop(normalAxis + 2, 3)] = v2 * hs.y
                };

                float t0 = i < 2 ? 0 : 1, t1 = Index.Loop(i + 1, 4) < 2 ? 0 : 1;
                var uv = new Vector2 { [0] = t0, [1] = t1 };

                vertexReceiver(i, new Vertex(pos, uv));
            }

            BuildRectangleTriangles(triangleReceiver, clockwise, 0, 1, 2, 3);
        }

        private static void BuildRectangleTriangles(Action<int, Triangle> triangleReceiver,
            bool clockwise, int bottomLeftIndex, int bottomRightIndex, int topRightIndex, int topLeftIndex)
        {
            if (clockwise)
            {
                triangleReceiver(0, new Triangle(bottomLeftIndex, bottomRightIndex, topRightIndex));
                triangleReceiver(1, new Triangle(topRightIndex, topLeftIndex, bottomLeftIndex));
            }
            else
            {
                triangleReceiver(0, new Triangle(bottomLeftIndex, topRightIndex, bottomRightIndex));
                triangleReceiver(1, new Triangle(topRightIndex, bottomLeftIndex, topLeftIndex));
            }
        }

        #endregion Rectangle
    }
}
