using System;

namespace ShuHai.NOctree
{
    public readonly struct NOctIndex : IEquatable<NOctIndex>
    {
        public static NOctIndex Invalid { get; } = new NOctIndex(Index.Invalid, Index.Invalid, Index.Invalid);

        public int I0 { get; }
        public int I1 { get; }
        public int I2 { get; }

        public NOctIndex(int i0, int i1, int i2)
        {
            I0 = i0;
            I1 = i1;
            I2 = i2;
        }

        public bool IsValid(int dimensionalChildCapacity)
        {
            return Index.IsValid(I0, dimensionalChildCapacity)
                   && Index.IsValid(I1, dimensionalChildCapacity)
                   && Index.IsValid(I2, dimensionalChildCapacity);
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
