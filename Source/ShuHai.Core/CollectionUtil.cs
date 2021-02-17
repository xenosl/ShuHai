using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    public static class CollectionUtil
    {
        /// <summary>
        ///     Indicates whether the specified collection is <see langword="null" /> or empty.
        /// </summary>
        /// <typeparam name="T">Type of collection element.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="collection" /> is <see langword="null" /> or a empty collection;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNullOrEmpty<T>(IEnumerable<T> collection)
        {
            switch (collection)
            {
                case null:
                    return true;
                case IReadOnlyCollection<T> readOnlyCollection:
                    return readOnlyCollection.Count == 0;
                case ICollection<T> genericCollection:
                    return genericCollection.Count == 0;
                case ICollection objectCollection:
                    return objectCollection.Count == 0;
            }

            using (var e = collection.GetEnumerator())
            {
                if (e.MoveNext())
                    return false;
            }
            return true;
        }
    }
}