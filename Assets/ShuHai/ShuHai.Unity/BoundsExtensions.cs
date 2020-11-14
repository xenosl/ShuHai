using UnityEngine;

namespace ShuHai.Unity
{
    public static class BoundsExtensions
    {
        #region Bounds 

        public static Bounds Encapsulated(this Bounds self, Vector3 point)
        {
            var b = new Bounds(self.center, self.size);
            b.Encapsulate(point);
            return b;
        }

        public static Bounds Encapsulated(this Bounds self, Bounds bounds)
        {
            var b = new Bounds(self.center, self.size);
            b.Encapsulate(bounds);
            return b;
        }

        #endregion Bounds 

        #region BoundsInt

        public static void Encapsulate(ref this BoundsInt self, Vector3Int point)
        {
            self.SetMinMax(Vector3Int.Min(self.min, point), Vector3Int.Max(self.max, point));
        }

        public static void Encapsulate(ref this BoundsInt self, BoundsInt bounds)
        {
            self.Encapsulate(bounds.min);
            self.Encapsulate(bounds.max);
        }

        public static BoundsInt Encapsulated(this BoundsInt self, Vector3Int point)
        {
            Encapsulate(ref self, point);
            return self;
        }

        public static BoundsInt Encapsulated(this BoundsInt self, BoundsInt bounds)
        {
            Encapsulate(ref self, bounds);
            return self;
        }

        #endregion BoundsInt
    }
}