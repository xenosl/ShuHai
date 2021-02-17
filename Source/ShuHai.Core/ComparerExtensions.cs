using System.Collections.Generic;

namespace ShuHai
{
    public static class ComparerExtensions
    {
        public static bool Equal<T>(this IComparer<T> self, T x, T y) => self.Compare(x, y) == 0;
        
        public static bool Greater<T>(this IComparer<T> self, T x, T y) => self.Compare(x, y) > 0;

        public static bool GreaterOrEqual<T>(this IComparer<T> self, T x, T y) => self.Compare(x, y) >= 0;

        public static bool Less<T>(this IComparer<T> self, T x, T y) => self.Compare(x, y) < 0;

        public static bool LessOrEqual<T>(this IComparer<T> self, T x, T y) => self.Compare(x, y) <= 0;
    }
}