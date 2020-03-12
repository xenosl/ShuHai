using UnityEngine;

namespace ShuHai.Unity
{
    public static class MatrixExtensions
    {
        public static void SetColumn(this ref Matrix4x4 self, int index, Vector3 col, float r3)
        {
            self.SetColumn(index, col.x, col.y, col.z, r3);
        }

        public static void SetColumn(this ref Matrix4x4 self, int index, float r0, float r1, float r2, float r3)
        {
            self[0, index] = r0;
            self[1, index] = r1;
            self[2, index] = r2;
            self[3, index] = r3;
        }

        public static void SetRow(this ref Matrix4x4 self, int index, Vector3 row, float c3)
        {
            self.SetRow(index, row.x, row.y, row.z, c3);
        }

        public static void SetRow(this ref Matrix4x4 self, int index, float c0, float c1, float c2, float c3)
        {
            self[index, 0] = c0;
            self[index, 1] = c1;
            self[index, 2] = c2;
            self[index, 3] = c3;
        }
    }
}