using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    using IColumnList = IReadOnlyList<SheetParser.Column>;
    using StringToColumnList = KeyValuePair<string, IReadOnlyList<SheetParser.Column>>;

    public class SheetParser : IReadOnlyList<SheetParser.Column>, IReadOnlyDictionary<string, IColumnList>
    {
        public sealed class Column : IEquatable<Column>
        {
            public SheetParser Owner { get; }

            public int Index { get; }

            public string Name { get; }

            public CellParser Parser { get; }

            public object DefaultValue { get; set; }

            public JToken Parse(object value) { return Parser.Parse(value, DefaultValue); }

            internal Column(SheetParser owner, int index, string name, CellParser parser)
            {
                Owner = owner;
                Index = index;
                Name = string.IsNullOrEmpty(name) ? index.ToString() : name;
                Parser = parser;
            }

            #region Equality

            public bool Equals(Column other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Owner == other.Owner && Index == other.Index;
            }

            public override bool Equals(object obj) { return obj is Column other && Equals(other); }

            public override int GetHashCode() { return HashCode.Combine(Owner.GetHashCode(), Index); }

            public static bool operator ==(Column l, Column r) { return EqualityComparer<Column>.Default.Equals(l, r); }
            public static bool operator !=(Column l, Column r) { return !(l == r); }

            #endregion Equality
        }

        public int ColumnCount => _columnList.Count;

        public Column this[int index] => _columnList[index];

        public IColumnList this[string name] => _columnDict[name];

        public SheetParser(IEnumerable<CellParser> columns)
            : this(columns.Select(p => ((string)null, p, (object)null))) { }

        public SheetParser(IEnumerable<(string Name, CellParser Parser)> columns)
            : this(columns.Select(c => (c.Name, c.Parser, (object)null))) { }

        public SheetParser(IEnumerable<(string Name, CellParser Parser, object defaultValue)> columns)
        {
            var list = new List<Column>();
            var dict = new Dictionary<string, List<Column>>();
            int index = 0;
            foreach (var (name, parser, defaultValue) in columns)
            {
                var column = new Column(this, index++, name, parser) { DefaultValue = defaultValue };
                list.Add(column);
                dict.Add(column.Name, column);
            }
            _columnList = list;
            _columnDict = dict.ToDictionary(p => p.Key, p => (IColumnList)p.Value);
        }

        public bool ContainsColumn(string columnName) { return _columnDict.ContainsKey(columnName); }

        public bool ContainsColumn(Column column)
        {
            return _columnDict.TryGetValue(column.Name, out var list) && list.Contains(column);
        }

        public IEnumerator<Column> GetEnumerator() { return _columnList.GetEnumerator(); }

        private readonly IColumnList _columnList;
        private readonly IReadOnlyDictionary<string, IColumnList> _columnDict;

        #region Explicit Implementations

        int IReadOnlyCollection<Column>.Count => _columnList.Count;
        int IReadOnlyCollection<StringToColumnList>.Count => _columnDict.Count;
        IEnumerable<string> IReadOnlyDictionary<string, IColumnList>.Keys => _columnDict.Keys;
        IEnumerable<IColumnList> IReadOnlyDictionary<string, IColumnList>.Values => _columnDict.Values;

        bool IReadOnlyDictionary<string, IColumnList>.ContainsKey(string key) { return _columnDict.ContainsKey(key); }

        bool IReadOnlyDictionary<string, IColumnList>.TryGetValue(string key, out IColumnList value)
        {
            return _columnDict.TryGetValue(key, out value);
        }

        IEnumerator<StringToColumnList> IEnumerable<StringToColumnList>.GetEnumerator()
        {
            return _columnDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion Explicit Implementations
    }
}
