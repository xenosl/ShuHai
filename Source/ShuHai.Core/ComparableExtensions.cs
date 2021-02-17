using System;

namespace ShuHai
{
    public static class ComparableExtensions
    {
        public static bool GreaterThan<T>(this IComparable<T> self, T value) => self.CompareTo(value) > 0;
        public static bool GreaterThanOrEqualTo<T>(this IComparable<T> self, T value) => self.CompareTo(value) >= 0;

        public static bool LessThan<T>(this IComparable<T> self, T value) => self.CompareTo(value) < 0;
        public static bool LessThanOrEqualTo<T>(this IComparable<T> self, T value) => self.CompareTo(value) <= 0;
    }
}