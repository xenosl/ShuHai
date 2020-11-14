using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Reflection;

namespace ShuHai.Edit
{
    using Set = HashSet<ObjectDataConverter>;
    using IROCollection = IReadOnlyCollection<ObjectDataConverter>;
    using Pair = KeyValuePair<Type, IReadOnlyCollection<ObjectDataConverter>>;

    public interface IReadOnlyObjectDataConverterCollection : IReadOnlyCollection<ObjectDataConverter>
    {
        IReadOnlyCollection<Type> ConvertTypes { get; }

        IROCollection this[Type convertType] { get; }

        bool TryGetConverters(Type convertType, out IROCollection converters);

        bool ContainsConvertType(Type convertType);
    }

    public static class ReadOnlyObjectDataConverterCollectionExtensions
    {
        public static ObjectDataConverter GetConverter(
            this IReadOnlyObjectDataConverterCollection self, Type convertType)
        {
            return self[convertType].FirstOrDefault();
        }

        public static bool TryGetConverter(
            this IReadOnlyObjectDataConverterCollection self, Type convertType, out ObjectDataConverter converter)
        {
            if (!self.TryGetConverters(convertType, out var converters) || converters.Count == 0)
            {
                converter = null;
                return false;
            }

            converter = converters.First();
            return true;
        }
    }

    public sealed class ObjectDataConverterCollection
        : IReadOnlyObjectDataConverterCollection, ICollection<ObjectDataConverter>
    {
        public int Count { get; private set; }

        public IReadOnlyCollection<Type> ConvertTypes => _dict.Keys;

        public IROCollection this[Type convertType] => _dict[convertType];

        public ObjectDataConverterCollection() { _dict = new Dictionary<Type, Set>(); }

        public ObjectDataConverterCollection(IEnumerable<ObjectDataConverter> converters)
        {
            _dict = converters.GroupBy(c => c.ConvertType).ToDictionary(g => g.Key, g => new Set(g));
        }

        public void Add(ObjectDataConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            _dict.Add(converter.ConvertType, converter);
            Count++;
        }

        public bool Remove(ObjectDataConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            Count--;
            return _dict.Remove(converter.ConvertType, converter);
        }

        public void Clear() { _dict.Clear(); }

        public bool Contains(ObjectDataConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return _dict.TryGetValue(converter.ConvertType, out var collection) && collection.Contains(converter);
        }

        public bool TryGetConverters(Type convertType, out IROCollection converters)
        {
            var got = _dict.TryGetValue(convertType, out var set);
            converters = set;
            return got;
        }

        public bool ContainsConvertType(Type convertType) { return _dict.ContainsKey(convertType); }

        private readonly Dictionary<Type, Set> _dict;

        #region ICollection

        bool ICollection<ObjectDataConverter>.IsReadOnly => false;

        void ICollection<ObjectDataConverter>.CopyTo(ObjectDataConverter[] array, int arrayIndex)
        {
            var collection = (IEnumerable<ObjectDataConverter>)this;
            int i = arrayIndex;
            foreach (var converter in collection)
            {
                if (i >= array.Length)
                    break;
                array[i++] = converter;
            }
        }

        #endregion ICollection

        #region IEnumerable

        IEnumerator<ObjectDataConverter> IEnumerable<ObjectDataConverter>.GetEnumerator()
        {
            foreach (var converters in _dict.Values)
            foreach (var converter in converters)
                yield return converter;
        }

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<ObjectDataConverter>)this).GetEnumerator(); }

        #endregion IEnumerable

        #region Instances

        /// <summary>
        ///     A collection contains default instance of every <see cref="ObjectDataConverter" /> type. The default instance of
        ///     certain <see cref="ObjectDataConverter" /> type is the static property or field named "Default" of the type.
        /// </summary>
        public static ObjectDataConverterCollection Default { get; private set; }

        private static void ResetDefault()
        {
            var rootType = typeof(ObjectDataConverter);
            var defaults = Assemblies.Instances.SelectMany(asm => asm.GetTypes())
                .Where(t => rootType.IsAssignableFrom(t))
                .Select(GetDefaultOrCreate)
                .Where(c => c != null);
            Default = new ObjectDataConverterCollection(defaults);
        }

        private static ObjectDataConverter GetDefaultOrCreate(Type type)
        {
            if (!AssignableMember.TryGet(type, "Default", BindingAttributes.DeclareStatic, out var member))
                return null;
            if (!typeof(ObjectDataConverter).IsAssignableFrom(member.ValueType))
                return null;
            return (ObjectDataConverter)member.GetValue(null);
        }

        #endregion Instances

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args) { ResetDefault(); }

        static ObjectDataConverterCollection()
        {
            ResetDefault();

            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
        }
    }
}