using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai.XConverts
{
    using DictItem = KeyValuePair<Type, XConverter>;

    public interface IReadOnlyXConverterCollection : IReadOnlyDictionary<Type, XConverter>
    {
        bool TryGet(Type convertType, out XConverter converter);

        bool Contains(XConverter converter);

        bool ContainsConvertType(Type convertType);
    }

    public class XConverterCollection : ICollection<XConverter>, IReadOnlyXConverterCollection
    {
        public int Count => _dict.Count;

        public XConverter this[Type convertType]
        {
            get
            {
                Ensure.Argument.NotNull(convertType, nameof(convertType));
                return _dict.GetValue(convertType);
            }
        }

        public XConverterCollection() { }

        public XConverterCollection(IEnumerable<XConverter> converters) { this.AddRange(converters); }

        public void Add(XConverter converter, bool replaceExisted)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));

            if (replaceExisted)
                _dict[converter.ConvertType] = converter;
            else
                _dict.Add(converter.ConvertType, converter);
        }

        public void Add(XConverter converter) { Add(converter, false); }

        public bool Remove(XConverter converter)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));
            return _dict.Remove(converter.ConvertType);
        }

        public void Clear() { _dict.Clear(); }

        public bool TryGet(Type convertType, out XConverter converter)
        {
            Ensure.Argument.NotNull(convertType, nameof(convertType));
            return _dict.TryGetValue(convertType, out converter);
        }

        public bool Contains(XConverter converter)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));
            return ContainsConvertType(converter.ConvertType);
        }

        public bool ContainsConvertType(Type convertType)
        {
            Ensure.Argument.NotNull(convertType, nameof(convertType));
            return _dict.ContainsKey(convertType);
        }

        public IEnumerator<XConverter> GetEnumerator() { return _dict.Values.GetEnumerator(); }

        private readonly Dictionary<Type, XConverter> _dict = new Dictionary<Type, XConverter>();

        #region Explicit Implementations

        bool ICollection<XConverter>.IsReadOnly => false;

        IEnumerable<Type> IReadOnlyDictionary<Type, XConverter>.Keys => _dict.Keys;
        IEnumerable<XConverter> IReadOnlyDictionary<Type, XConverter>.Values => _dict.Values;

        bool IReadOnlyDictionary<Type, XConverter>.ContainsKey(Type key) { return ContainsConvertType(key); }

        bool IReadOnlyDictionary<Type, XConverter>.TryGetValue(Type key, out XConverter value)
        {
            return TryGet(key, out value);
        }

        void ICollection<XConverter>.CopyTo(XConverter[] array, int arrayIndex) { throw new NotSupportedException(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        IEnumerator<DictItem> IEnumerable<DictItem>.GetEnumerator() { return _dict.GetEnumerator(); }

        #endregion Explicit Implementations
    }
}