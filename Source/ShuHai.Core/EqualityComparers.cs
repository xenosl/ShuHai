using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    public class FloatAlmostEqualComparer : IEqualityComparer<float>
    {
        public static FloatAlmostEqualComparer Default { get; } = new FloatAlmostEqualComparer();

        public float Tolerance;

        public FloatAlmostEqualComparer(float tolerance = float.Epsilon) { Tolerance = tolerance; }

        public bool Equals(float l, float r) { return l.AlmostEquals(r, Tolerance); }

        public int GetHashCode(float value) { return value.GetHashCode(); }
    }

    public class DoubleAlmostEqualComparer : IEqualityComparer<double>
    {
        public static DoubleAlmostEqualComparer Default { get; } = new DoubleAlmostEqualComparer();

        public double Tolerance;

        public DoubleAlmostEqualComparer(double tolerance = double.Epsilon) { Tolerance = tolerance; }

        public bool Equals(double l, double r) { return l.AlmostEquals(r, Tolerance); }

        public int GetHashCode(double value) { return value.GetHashCode(); }
    }

    public class SequentialEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public static SequentialEqualityComparer<T> Default { get; } = new SequentialEqualityComparer<T>();

        public IEqualityComparer<T> ElementComparer { get; set; }

        public SequentialEqualityComparer(IEqualityComparer<T> elementComparer = null)
        {
            ElementComparer = elementComparer;
        }

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            bool xn = ReferenceEquals(x, null), yn = ReferenceEquals(y, null);
            if (xn && yn)
                return true;
            if (xn || yn)
                return false;

            var elementComparer = ElementComparer ?? EqualityComparer<T>.Default;
            using (var xe = x.GetEnumerator())
            using (var ye = y.GetEnumerator())
            {
                while (xe.MoveNext() && ye.MoveNext())
                {
                    if (!elementComparer.Equals(xe.Current, ye.Current))
                        return false;
                }
            }

            return true;
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            int h = 0;
            foreach (var e in obj)
                h = Hash.Combine(h, e.GetHashCode());
            return h;
        }
    }

    public static class EqualityComparerExtensions
    {
        public static IEqualityComparer<T> ToGeneric<T>(this IEqualityComparer self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return new ObjectEqualityComparer<T>(self);
        }

        private class ObjectEqualityComparer<T> : IEqualityComparer<T>
        {
            public readonly IEqualityComparer Comparer;

            public ObjectEqualityComparer(IEqualityComparer comparer) { Comparer = comparer; }

            public bool Equals(T x, T y) { return Comparer.Equals(x, y); }

            public int GetHashCode(T obj) { return Comparer.GetHashCode(obj); }
        }
    }
}