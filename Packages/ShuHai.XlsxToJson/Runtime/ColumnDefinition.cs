using System;
using System.Collections;
using System.Collections.Generic;

namespace ShuHai.XlsxToJson
{
    public enum CellValueType
    {
        String,
        Integer,
        Float,
        Boolean,
        Null
    }

    public struct ColumnMapping
    {
        public const CellValueType DefaultValueType = CellValueType.String;

        public int XlsxColumnIndex { get; set; }

        public string JsonKeyName { get; set; }

        public CellValueType ValueType { get; set; }

        public object FallbackValue { get; set; }

        public ColumnMapping(int xlsxColumnIndex, string jsonKeyName,
            CellValueType valueType = DefaultValueType, object fallbackValue = default)
        {
            XlsxColumnIndex = xlsxColumnIndex;
            JsonKeyName = jsonKeyName;
            ValueType = valueType;
            FallbackValue = fallbackValue;
        }
    }

    public class ColumnDefinition : IReadOnlyList<ColumnMapping>
    {
        public int Count => _list.Count;

        public ColumnMapping this[int index]
        {
            get => _list[index];
            set
            {
                if (!Index.IsValid(index, _list.Count))
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (value.XlsxColumnIndex != index)
                    throw new ArgumentException("XlsxColumnIndex and index argument does not match.");
                _list[index] = value;
            }
        }

        public ColumnDefinition() { }

        public ColumnDefinition(IEnumerable<(string Key, CellValueType ValueType)> jsonProperties)
        {
            foreach (var prop in jsonProperties)
                AddColumnMapping(prop.Key, prop.ValueType);
        }

        public ColumnMapping AddColumnMapping(string jsonKeyName,
            CellValueType valueType = ColumnMapping.DefaultValueType)
        {
            Ensure.Argument.NotNullOrEmpty(jsonKeyName, nameof(jsonKeyName));

            int index = _list.Count;
            var mapping = new ColumnMapping(index, jsonKeyName, valueType);
            _list.Add(mapping);
            _nameToIndex.Add(jsonKeyName, index);
            return mapping;
        }

        public bool TryGetColumnMapping(int xlsxColumnIndex, out ColumnMapping mapping)
        {
            if (!Index.IsValid(xlsxColumnIndex, _list.Count))
            {
                mapping = default;
                return false;
            }
            mapping = _list[xlsxColumnIndex];
            return true;
        }

        public bool TryGetColumnMapping(string jsonKeyName, out ColumnMapping mapping)
        {
            Ensure.Argument.NotNullOrEmpty(jsonKeyName, nameof(jsonKeyName));

            if (!_nameToIndex.TryGetValue(jsonKeyName, out var index))
            {
                mapping = default;
                return false;
            }

            mapping = _list[index];
            return true;
        }

        public int GetXlsxColumnIndex(string jsonKeyName)
        {
            Ensure.Argument.NotNullOrEmpty(jsonKeyName, nameof(jsonKeyName));

            return _nameToIndex.TryGetValue(jsonKeyName, out var index) ? index : Index.Invalid;
        }

        public IEnumerator<ColumnMapping> GetEnumerator() { return _list.GetEnumerator(); }

        private readonly List<ColumnMapping> _list = new List<ColumnMapping>();
        private readonly Dictionary<string, int> _nameToIndex = new Dictionary<string, int>();

        #region Explicit Implementations

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_list).GetEnumerator(); }

        #endregion Explicit Implementations
    }
}
