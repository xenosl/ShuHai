using System;
using System.Collections.Generic;

namespace ShuHai
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        public static T Clamp<T>(this T value, T min, T max, IComparer<T> comparer)
        {
            Ensure.Argument.NotNull(comparer, nameof(comparer));

            if (comparer.Compare(value, min) < 0)
                return min;
            if (comparer.Compare(value, max) > 0)
                return max;
            return value;
        }
    }
}