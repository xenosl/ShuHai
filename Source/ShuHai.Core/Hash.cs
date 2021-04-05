using System.Collections.Generic;
using System.Linq;

namespace ShuHai
{
    public static class Hash
    {
        #region Calculate

        public static int Calculate<T1, T2>(T1 o1, T2 o2) { return Combine(Calculate(o1), Calculate(o2)); }

        public static int Calculate<T1, T2, T3>(T1 o1, T2 o2, T3 o3)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3));
        }

        public static int Calculate<T1, T2, T3, T4>(T1 o1, T2 o2, T3 o3, T4 o4)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3), Calculate(o4));
        }

        public static int Calculate<T1, T2, T3, T4, T5>(T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3), Calculate(o4), Calculate(o5));
        }

        public static int Calculate<T1, T2, T3, T4, T5, T6>(T1 o1, T2 o2, T3 o3, T4 o4, T5 o5, T6 o6)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3), Calculate(o4), Calculate(o5), Calculate(o6));
        }

        public static int Calculate<T1, T2, T3, T4, T5, T6, T7>(T1 o1, T2 o2, T3 o3, T4 o4, T5 o5, T6 o6, T7 o7)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3),
                Calculate(o4), Calculate(o5), Calculate(o6), Calculate(o7));
        }

        public static int Calculate<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 o1, T2 o2, T3 o3, T4 o4, T5 o5, T6 o6, T7 o7, T8 o8)
        {
            return Combine(Calculate(o1), Calculate(o2), Calculate(o3), Calculate(o4),
                Calculate(o5), Calculate(o6), Calculate(o7), Calculate(o8));
        }

        public static int Calculate<T>(T value) => Calculate(value, null);

        /// <summary>
        ///     Get hash code of specified value with specified comparer if possible.
        /// </summary>
        /// <remarks>
        ///     This method is not recommended if performance is critical. Actually you'd better only use it in an immutable type
        ///     which the hash code only calculated once in the constructor.
        /// </remarks>
        public static int Calculate<T>(T value, IEqualityComparer<T> comparer)
        {
            return comparer?.GetHashCode(value) ?? EqualityComparer<T>.Default.GetHashCode(value);
        }

        public static int CalculateSequence<T>(IEnumerable<T> sequence)
        {
            int h = 0;
            using (var e = sequence.GetEnumerator())
            {
                while (e.MoveNext())
                    h = Combine(h, e.Current?.GetHashCode() ?? 0);
            }
            return h;
        }

        #endregion Calculate

        #region Combine

        public static int Combine(int h1, int h2)
        {
            if (h1 == 0)
                return h2;
            if (h2 == 0)
                return h1;
            return unchecked(((h1 << 5) + h1) ^ h2);
        }

        public static int Combine(int h1, int h2, int h3) { return Combine(Combine(h1, h2), h3); }

        public static int Combine(int h1, int h2, int h3, int h4) { return Combine(Combine(h1, h2, h3), h4); }

        public static int Combine(int h1, int h2, int h3, int h4, int h5)
        {
            return Combine(Combine(h1, h2, h3, h4), h5);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return Combine(Combine(h1, h2, h3, h4, h5), h6);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return Combine(Combine(h1, h2, h3, h4, h5, h6), h7);
        }

        public static int Combine(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return Combine(Combine(h1, h2, h3, h4, h5, h6, h7), h8);
        }

        public static int Combine(IEnumerable<int> values)
        {
            int h = 0;
            using (var e = values.GetEnumerator())
                h = Combine(h, e.Current);
            return h;
        }

        #endregion Combine
    }
}
