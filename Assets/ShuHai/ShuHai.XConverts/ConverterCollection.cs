using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai.XConverts
{
    using DictItem = KeyValuePair<Type, XConverter>;

    public interface IReadOnlyConverterCollection : IReadOnlyCollection<XConverter>
    {
        XConverter this[Type targetType] { get; }

        bool Contains(XConverter converter);

        bool ContainsTargetType(Type targetType);
    }

    public class ConverterCollection : ICollection<XConverter>, IReadOnlyConverterCollection
    {
        public int Count => _dict.Count;

        public XConverter this[Type targetType]
        {
            get
            {
                Ensure.Argument.NotNull(targetType, nameof(targetType));
                return _dict.GetValue(targetType);
            }
        }

        public ConverterCollection() { }

        public ConverterCollection(IEnumerable<XConverter> converters) { this.AddRange(converters); }

        public void Add(XConverter converter)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));
            _dict.Add(converter.BaseConvertType, converter);
        }

        public bool Remove(XConverter converter)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));
            return _dict.Remove(converter.BaseConvertType);
        }

        public void Clear() { _dict.Clear(); }

        public bool Contains(XConverter converter)
        {
            Ensure.Argument.NotNull(converter, nameof(converter));
            return ContainsTargetType(converter.BaseConvertType);
        }

        public bool ContainsTargetType(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            return _dict.ContainsKey(type);
        }

        public IEnumerator<XConverter> GetEnumerator() { return _dict.Values.GetEnumerator(); }

        private readonly Dictionary<Type, XConverter> _dict = new Dictionary<Type, XConverter>();

        #region Explicit Implementations

        bool ICollection<XConverter>.IsReadOnly => false;
        void ICollection<XConverter>.CopyTo(XConverter[] array, int arrayIndex) { throw new NotSupportedException(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion Explicit Implementations
    }
}