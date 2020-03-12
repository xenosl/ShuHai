using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    public static class CollectionExtensions
    {
        #region Enumerate

        /// <summary>
        ///     Performs the specified action on each element of specified collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="self">The collection itself.</param>
        /// <param name="action">Action to perform.</param>
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(action, nameof(action));

            foreach (var item in self)
                action(item);
        }

        /// <summary>
        ///     Enumerate a <see cref="IEnumerator" /> as <see cref="IEnumerable" />.
        /// </summary>
        public static IEnumerable ToEnumerable(this IEnumerator self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            while (self.MoveNext())
                yield return self.Current;
        }

        /// <summary>
        ///     Enumerate a <see cref="IEnumerator{T}" /> as <see cref="IEnumerable{T}" />.
        /// </summary>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            while (self.MoveNext())
                yield return self.Current;
        }

        #endregion Enumerate

        #region Collection

        /// <summary>
        ///     Add items from sepcified enumeration to the current collection instance.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(items, "items");

            foreach (var item in items)
                self.Add(item);
        }

        public static int AddRange<T>(this ISet<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(items, "items");

            var addCount = 0;
            foreach (var item in items)
            {
                if (self.Add(item))
                    addCount++;
            }
            return addCount;
        }

        public static int RemoveRange<T>(this ICollection<T> self, IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(items, "items");

            var removeCount = 0;
            foreach (var item in items)
            {
                if (self.Remove(item))
                    removeCount++;
            }
            return removeCount;
        }

        #endregion Collection

        #region List

        /// <summary>
        ///     Get a value from list at specific <paramref name="index" />.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="self">The list instance.</param>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="valueOnOutOfRange">The value that returns when <paramref name="index" /> out of range.</param>
        /// <returns>The element at the specified index.</returns>
        public static T At<T>(this IReadOnlyList<T> self, int index, T valueOnOutOfRange = default)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (!Index.IsValid(index, self.Count))
                return valueOnOutOfRange;
            return self[index];
        }

        public static T LoopedAt<T>(this IReadOnlyList<T> self, int index, T valueOnOutOfRange = default)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.At(index = Index.Loop(index, self.Count), valueOnOutOfRange);
        }

        /// <summary>
        ///     Searches for the specified item in the collection and returns the zero-based index of the first occurence.
        /// </summary>
        /// <param name="self">The collection to search.</param>
        /// <param name="item">The item object located in the <paramref name="self" />.</param>
        /// <typeparam name="T">Type of the elements in the collection.</typeparam>
        /// <returns>
        ///     The zero-based index of the first occurrence of item within the specified collection, if found;
        ///     otherwise, -1.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> self, T item)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            var index = 0;
            foreach (var e in self)
            {
                if (EqualityComparer<T>.Default.Equals(e, item))
                    return index;
                ++index;
            }
            return -1;
        }

        /// <summary>
        ///     Searches for the item that matches the specified predicate in the specified collection and returns the
        ///     zero-based index of the first occurence.
        /// </summary>
        /// <param name="self">The collection to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="T">The type of elements in collection.</typeparam>
        /// <returns>
        ///     The zero-based index of the first occurrence of item that matches the specified predicate, if found;
        ///     otherwise, -1.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> self, Func<T, bool> predicate)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(predicate, nameof(predicate));

            var index = 0;
            foreach (var e in self)
            {
                if (predicate(e))
                    return index;
                ++index;
            }
            return -1;
        }

        /// <summary>
        ///     Performs the specified action on each element of list <paramref name="self" />.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="self">The list instance.</param>
        /// <param name="func">
        ///     The action to perform. Returns <see langword="true" /> to continue or <see langword="false" /> to break
        ///     the enumeration.
        /// </param>
        /// <param name="reverseOrder">Is actions performed in reverse order of the list.</param>
        public static void ForEach<T>(this IReadOnlyList<T> self, Func<int, T, bool> func, bool reverseOrder = false)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(func, nameof(func));

            if (reverseOrder)
            {
                for (int i = self.Count - 1; i >= 0; --i)
                {
                    if (!func(i, self[i]))
                        break;
                }
            }
            else
            {
                for (var i = 0; i < self.Count; ++i)
                {
                    if (!func(i, self[i]))
                        break;
                }
            }
        }

        /// <summary>
        ///     Performs the specified action on elements specified by pair of index in the specified list.
        /// </summary>
        /// <param name="self">The list instance.</param>
        /// <param name="startIndex">The index of the element from which to starts with.</param>
        /// <param name="endIndex">The index of the element from which to ends with.</param>
        /// <param name="func">
        ///     The action to perform. Returns <see langword="true" /> to continue or <see langword="false" /> to break
        ///     the enumeration.
        /// </param>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <remarks>
        ///     The method support both forward enumeration and backward enumeration on list elements, this is determined
        ///     by the value of <paramref name="startIndex" /> and <paramref name="endIndex" />. The enumeration is forward
        ///     if <paramref name="endIndex" /> is greater than <paramref name="startIndex" />; otherwise the enumeration
        ///     is backward.
        /// </remarks>
        public static void ForEach<T>(this IReadOnlyList<T> self,
            int startIndex, int endIndex, Func<int, T, bool> func)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.ValidIndex(startIndex, self.Count, nameof(startIndex));
            Ensure.Argument.ValidIndex(endIndex, self.Count, nameof(endIndex));
            Ensure.Argument.NotNull(func, nameof(func));

            if (startIndex == endIndex)
            {
                func(startIndex, self[startIndex]);
                return;
            }

            int step = Math.Sign(endIndex - startIndex);
            int end = step > 0 ? endIndex + 1 : endIndex - 1;
            for (int i = startIndex; i != end; i += step)
            {
                if (!func(i, self[i]))
                    break;
            }
        }

        #endregion List

        #region Dictionary

        /// <summary>
        ///     Get element with specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="self">The dictionary instance.</param>
        /// <param name="key">The key of the element to get.</param>
        /// <param name="fallback">Value that returns when element with <paramref name="key" /> not found.</param>
        /// <returns>
        ///     Essence with specified <paramref name="key" />, if found; otherwise, <paramref name="fallback" />.
        /// </returns>
        public static TValue GetValue<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> self, TKey key, TValue fallback = default)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (!self.TryGetValue(key, out var value))
                value = fallback;
            return value;
        }

        public static void RemoveRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<TKey> keys)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(keys, nameof(keys));

            foreach (var key in keys)
                self.Remove(key);
        }

        #region Multi-Value

        public static TValueCollection AddRange<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, IEnumerable<TValue> values,
            Func<TValueCollection> valueCollectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(valueCollectionFactory, nameof(valueCollectionFactory));

            if (!self.TryGetValue(key, out var valueCollection))
            {
                valueCollection = valueCollectionFactory();
                if (valueCollection == null)
                    throw new NullReferenceException("Failed to create value collection.");
                self.Add(key, valueCollection);
            }
            valueCollection.AddRange(values);

            return valueCollection;
        }

        public static TValueCollection AddRange<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, IEnumerable<TValue> values)
            where TValueCollection : ICollection<TValue>, new()
        {
            return AddRange(self, key, values, NewValueCollection<TValueCollection>);
        }

        public static TValueCollection Add<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value,
            Func<TValueCollection> valueCollectionFactory)
            where TValueCollection : ICollection<TValue>
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(valueCollectionFactory, nameof(valueCollectionFactory));

            if (!self.TryGetValue(key, out var valueCollection))
            {
                valueCollection = valueCollectionFactory();
                if (valueCollection == null)
                    throw new NullReferenceException("Failed to create value collection.");
                self.Add(key, valueCollection);
            }
            valueCollection.Add(value);

            return valueCollection;
        }

        public static TValueCollection Add<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value)
            where TValueCollection : ICollection<TValue>, new()
        {
            return Add(self, key, value, NewValueCollection<TValueCollection>);
        }

        private static TValueCollection NewValueCollection<TValueCollection>()
            where TValueCollection : new()
        {
            return new TValueCollection();
        }

        public static bool Remove<TKey, TValue, TValueCollection>(
            this IDictionary<TKey, TValueCollection> self, TKey key, TValue value)
            where TValueCollection : ICollection<TValue>
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (!self.TryGetValue(key, out var values))
                return false;

            bool removed = values.Remove(value);
            if (removed && values.Count == 0)
                self.Remove(key);
            return removed;
        }

        #endregion Multi-Value

        #endregion Dictionary
    }
}