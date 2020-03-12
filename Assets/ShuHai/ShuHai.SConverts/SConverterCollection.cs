using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShuHai.SConverts.Converters;

namespace ShuHai.SConverts
{
    public class SConverterCollection<T> : ICollection<T>
        where T : ISConverter
    {
        public int Count { get; private set; }

        public SConverterCollection() : this(SConverterPriorityComparer<T>.Instance) { }

        public SConverterCollection(IComparer<T> priorityComparer)
            : this(Array.Empty<T>(), priorityComparer) { }

        public SConverterCollection(IEnumerable<T> collection, IComparer<T> priorityComparer)
        {
            this.priorityComparer = priorityComparer;
            dict = new Dictionary<Type, HashSet<T>>();

            this.AddRange(collection);
        }

        public bool Add(T item)
        {
            Ensure.Argument.NotNull(item, nameof(item));

            var targetType = item.TargetType;
            if (!dict.TryGetValue(targetType, out var set))
            {
                set = new HashSet<T>();
                dict.Add(targetType, set);
            }

            bool added = set.Add(item);
            if (added)
                Count++;
            return added;
        }

        public bool Remove(T item)
        {
            Ensure.Argument.NotNull(item, nameof(item));

            bool removed = dict.TryGetValue(item.TargetType, out var set) && set.Remove(item);
            if (removed)
                Count--;
            return removed;
        }

        public void Clear()
        {
            dict.Clear();
            Count = 0;
        }

        public bool Contains(T item)
        {
            Ensure.Argument.NotNull(item, nameof(item));
            return dict.TryGetValue(item.TargetType, out var set) && set.Contains(item);
        }

        /// <summary>
        ///     Get the most appropriate converter for specified target type in current collection.
        /// </summary>
        /// <param name="targetType">The type to convert.</param>
        /// <param name="useBaseIfNecessary">
        ///     Use base type of specified target type as target type if there is no converter for the specified target
        ///     type.
        /// </param>
        /// <param name="fallback">The fallback instance used if the converter is not found.</param>
        public T GetByTargetType(Type targetType, bool useBaseIfNecessary = true, T fallback = default)
        {
            Ensure.Argument.NotNull(targetType, nameof(targetType));
            return TryGetByTargetType(targetType, useBaseIfNecessary, out var converter) ? converter : fallback;
        }

        public bool TryGetByTargetType(Type targetType, out T converter)
        {
            return TryGetByTargetType(targetType, true, out converter);
        }

        /// <summary>
        ///     Try to get the most appropriate converter for specified target type in current collection.
        /// </summary>
        /// <param name="targetType">The type to convert.</param>
        /// <param name="useBaseIfNecessary">
        ///     Use base type of specified target type as target type if there is no converter for the specified target
        ///     type.
        /// </param>
        /// <param name="converter">
        ///     The converter instance which is the most appropriate one to convert the specified type.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the converter successfully got; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetByTargetType(Type targetType, bool useBaseIfNecessary, out T converter)
        {
            Ensure.Argument.NotNull(targetType, nameof(targetType));

            if (TryGetByTargetTypeImpl(targetType, out converter))
                return true;

            if (!useBaseIfNecessary)
                return false;

            var t = targetType;
            while (t != null)
            {
                if (TryGetByTargetTypeImpl(t, out converter))
                    return true;
                t = t.BaseType;
            }
            return false;
        }

        private bool TryGetByTargetTypeImpl(Type targetType, out T converter)
        {
            converter = default;
            if (!dict.TryGetValue(targetType, out var set))
                return false;

            var converters = set.Where(c => c.CanConvert(targetType)).OrderByDescending(c => c, priorityComparer);
            if (!converters.Any())
                return false;

            converter = converters.First();
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var (_, set) in dict)
            {
                foreach (var converter in set)
                    yield return converter;
            }
        }

        private readonly Dictionary<Type, HashSet<T>> dict;

        private readonly IComparer<T> priorityComparer;

        #region Explicit Implementations

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) { Add(item); }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            throw new NotSupportedException($@"""CopyTo"" for ""{GetType().FullName}"" is not supported.");
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion Explicit Implementations
    }

    public class SConverterCollection : SConverterCollection<ISConverter>
    {
        public static readonly SConverterCollection Default = new SConverterCollection
        {
            CharConverter.Instance, BooleanConverter.Instance,
            SByteConverter.Instance, ByteConverter.Instance,
            Int16Converter.Instance, UInt16Converter.Instance,
            Int32Converter.Instance, UInt32Converter.Instance,
            Int64Converter.Instance, UInt64Converter.Instance,
            SingleConverter.Instance, DoubleConverter.Instance,
            IntPtrConverter.Instance, UIntPtrConverter.Instance,
            TypeConverter.Instance, StringConverter.Instance, DecimalConverter.Instance
        };
    }
}