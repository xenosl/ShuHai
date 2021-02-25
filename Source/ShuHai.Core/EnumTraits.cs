using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    using StringList = List<string>;

    public static class EnumTraits<T>
        where T : Enum
    {
        public static Type Type { get; } = typeof(T);

        public static int ElementCount => Values.Count;

        public static IReadOnlyList<string> Names { get; }

        public static IReadOnlyList<T> Values { get; }

        public static IReadOnlyDictionary<T, int> ValueToIndex { get; }

        public static IReadOnlyDictionary<string, int> NameToIndex { get; }

        public static IReadOnlyDictionary<string, T> NameToValue { get; }

        public static IReadOnlyDictionary<T, IReadOnlyCollection<string>> ValueToName { get; }


        static EnumTraits()
        {
            var nameToField = new Dictionary<string, FieldInfo>();
            foreach (var f in Type.GetFields())
            {
                if (f.IsLiteral && !f.IsInitOnly && !f.IsDefined(typeof(ObsoleteAttribute), false))
                    nameToField.Add(f.Name, f);
            }

            var names = new StringList();
            var values = new List<T>();
            var nameToValue = new Dictionary<string, T>();
            var valueToName = new Dictionary<T, StringList>();
            foreach (string name in Enum.GetNames(Type))
            {
                if (!nameToField.TryGetValue(name, out var field))
                    continue;

                var value = (T)field.GetRawConstantValue();
                names.Add(name);
                values.Add(value);
                nameToValue.Add(name, value);
                valueToName.Add(value, name);
            }
            Names = names;
            Values = values;
            NameToValue = nameToValue;
            ValueToName = valueToName.ToDictionary(p => p.Key, p => (IReadOnlyCollection<string>)p.Value);

            var nameToIndex = new Dictionary<string, int>();
            var valueToIndex = new Dictionary<T, int>();
            for (int i = 0; i < Values.Count; ++i)
            {
                valueToIndex[Values[i]] = i; // Value may duplicate.
                nameToIndex.Add(Names[i], i);
            }
            ValueToIndex = valueToIndex;
            NameToIndex = nameToIndex;
        }
    }
}
