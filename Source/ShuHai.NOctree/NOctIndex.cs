using System;

namespace ShuHai.NOctree
{
    public struct NOctIndex : IEquatable<NOctIndex>
    {
        public static NOctIndex Zero { get; } = new NOctIndex();

        public int I0;
        public int I1;
        public int I2;

        public NOctIndex(int i0, int i1, int i2)
        {
            I0 = i0;
            I1 = i1;
            I2 = i2;
        }

        public bool IsValid(int segmentCount)
        {
            return Index.IsValid(I0, segmentCount)
                && Index.IsValid(I1, segmentCount)
                && Index.IsValid(I2, segmentCount);
        }

        public bool Equals(NOctIndex other) { return I0 == other.I0 && I1 == other.I1 && I2 == other.I2; }

        public override bool Equals(object obj) { return obj is NOctIndex other && Equals(other); }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = I0;
                hashCode = (hashCode * 397) ^ I1;
                hashCode = (hashCode * 397) ^ I2;
                return hashCode;
            }
        }

        public static bool operator ==(NOctIndex l, NOctIndex r) { return l.Equals(r); }
        public static bool operator !=(NOctIndex l, NOctIndex r) { return !(l == r); }
    }
}
