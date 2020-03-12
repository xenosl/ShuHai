using System.Collections.Generic;
using System.Linq;

namespace ShuHai
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T value) { yield return value; }

        public static IEnumerable<T> Concat<T>(this T value, IEnumerable<T> sequence)
        {
            return ToEnumerable(value).Concat(sequence);
        }
    }
}