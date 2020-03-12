using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShuHai.SConverts;
using ShuHai.SConverts.Converters;
using UnityEngine;

namespace ShuHai.Unity
{
    using Option = KeyValuePair<string, object>;

    [CreateAssetMenu(fileName = "Config", menuName = "ShuHai/Config", order = 50)]
    public class Config : ScriptableObject, IDictionary<string, object>, ISerializationCallbackReceiver
    {
        #region Creation

        public static Config Create() { return Create(Enumerable.Empty<Option>()); }

        public static Config Create(IEnumerable<KeyValuePair<string, object>> values)
        {
            var config = CreateInstance<Config>();

            foreach (var p in values)
                config.Add(p.Key, p.Value);

            return config;
        }

        protected Config() { }

        #endregion Creation

        #region Dictionary

        public int Count => _dict.Count;

        public ICollection<string> Keys => _dict.Keys;

        public ICollection<object> Values => _dict.Values;

        public object this[string key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }

        public void Add(string key, object value) { _dict.Add(key, value); }
        public void Add(Option item) { Collection.Add(item); }

        public bool Remove(string key) { return _dict.Remove(key); }
        public bool Remove(Option item) { return Collection.Remove(item); }

        public void Clear() { _dict.Clear(); }

        public bool ContainsKey(string key) { return _dict.ContainsKey(key); }
        public bool Contains(Option item) { return Collection.Contains(item); }

        public bool TryGetValue(string key, out object value) { return _dict.TryGetValue(key, out value); }

        public void CopyTo(Option[] array, int arrayIndex) { Collection.CopyTo(array, arrayIndex); }

        public IEnumerator<Option> GetEnumerator() { return _dict.GetEnumerator(); }

        private ICollection<Option> Collection => _dict;
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        #region Explicit Implementations

        bool ICollection<Option>.IsReadOnly => Collection.IsReadOnly;

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_dict).GetEnumerator(); }

        #endregion Explicit Implementations

        #endregion Dictionary

        #region Serialization

        [Serializable]
        private struct OptionStrings
        {
            public string Key;
            public string ValueType;
            public string Value;
        }

        [SerializeField] private List<OptionStrings> list = new List<OptionStrings>();

        private static OptionStrings ValuePairToItem(Option pair)
        {
            var value = pair.Value;
            var type = value?.GetType();
            return new OptionStrings
            {
                Key = pair.Key,
                ValueType = TypeConverter.Instance.ToString(type),
                Value = SConvert.ToString(value)
            };
        }

        private static object ItemToValuePair(OptionStrings item)
        {
            var type = TypeConverter.Instance.ToValue(item.ValueType);
            return SConvert.ToValue(type, item.Value);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { list = _dict.Select(ValuePairToItem).ToList(); }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _dict = list.ToDictionary(e => e.Key, ItemToValuePair);
        }

        #endregion Serialization
    }
}