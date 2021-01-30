using System;

namespace ShuHai.Edit
{
    public sealed class ValueData : Data, IEquatable<ValueData>
    {
        public static ValueData Null { get; } = new ValueData(null);

        public static ValueData Create(object value)
        {
            if (!CanCreate(value))
                throw new ArgumentException($"Failed to create data for {value}.");
            return CreateImpl(value);
        }

        public static bool TryCreate(object value, out ValueData data)
        {
            var type = value?.GetType();
            if (!IsValidType(type))
            {
                data = null;
                return false;
            }
            data = CreateImpl(value);
            return true;
        }

        private static ValueData CreateImpl(object value)
        {
            return ReferenceEquals(value, null) ? Null : new ValueData(value);
        }

        public static bool CanCreate(object value) { return IsValidType(value?.GetType()); }

        /// <summary>
        ///     Indicates whether the specified type is a valid type for creating <see cref="ValueData" />.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified type is an valid type for creating <see cref="ValueData" />; otherwise
        ///     <see langword="false" />.
        /// </returns>
        public static bool IsValidType(Type type)
        {
            return type == null || type.IsValueType || type == typeof(string) || typeof(Type).IsAssignableFrom(type);
        }

        public object Value { get; }

        private ValueData(object value) { Value = value; }

        #region Equality

        public override bool Equals(Data other) { return Equals(other as ValueData); }

        public bool Equals(ValueData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ValueData other && Equals(other);
        }

        public override int GetHashCode() { return !ReferenceEquals(Value, null) ? Value.GetHashCode() : 0; }

        #endregion Equality
    }
}