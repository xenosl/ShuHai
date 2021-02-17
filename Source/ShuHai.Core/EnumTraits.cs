using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai
{
    using StringList = List<string>;

    public static class EnumTraits<T>
        where T : Enum
    {
        public static readonly Type Type = typeof(T);

        public static int ElementCount => _values.Count;

        public static IReadOnlyList<string> Names => _names;

        public static IReadOnlyList<T> Values => _values;

        public static int IndexOf(T value) { return _valueIndices.GetValue(value, Index.Invalid); }

        public static T GetValue(string name) { return _nameToValue[name]; }

        public static bool IsValid(string name) { return _nameToValue.ContainsKey(name); }

        /// <summary>
        ///     Get a value indicates whether the specified enum value is a valid element defined in <see cref="T" />.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value is a valid value defined in <see cref="T" />; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        /// <remarks>
        ///     A valid enum value means that the value equals any member of <see cref="T" /> and the value is not
        ///     declared as obsoleted (which marked with <see cref="ObsoleteAttribute" />).
        /// </remarks>
        public static bool IsValid(T value) { return _valueToNames.ContainsKey(value); }

        private static readonly Dictionary<string, T> _nameToValue = new Dictionary<string, T>();
        private static readonly Dictionary<T, StringList> _valueToNames = new Dictionary<T, StringList>();
        private static readonly StringList _names = new StringList();
        private static readonly List<T> _values = new List<T>();
        private static readonly Dictionary<T, int> _valueIndices = new Dictionary<T, int>();

        static EnumTraits()
        {
            var nameToField = new Dictionary<string, FieldInfo>();
            foreach (var f in Type.GetFields())
            {
                if (f.IsLiteral && !f.IsInitOnly && !f.IsDefined(typeof(ObsoleteAttribute), false))
                    nameToField.Add(f.Name, f);
            }

            foreach (string name in Enum.GetNames(Type))
            {
                if (!nameToField.TryGetValue(name, out var field))
                    continue;

                var value = (T)field.GetRawConstantValue();
                _names.Add(name);
                _values.Add(value);
                _nameToValue.Add(name, value);
                _valueToNames.Add(value, name);
            }

            for (int i = 0; i < _values.Count; ++i)
                _valueIndices.Add(_values[i], i);
        }
    }
}
