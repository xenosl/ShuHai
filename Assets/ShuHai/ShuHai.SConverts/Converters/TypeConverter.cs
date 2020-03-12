using System;

namespace ShuHai.SConverts.Converters
{
    public class TypeConverter : SConverter<Type>
    {
        public static readonly TypeConverter Instance = new TypeConverter();

        public override string ToString(Type value) { return value != null ? value.FullName : string.Empty; }

        public override Type ToValue(string str) { return !string.IsNullOrEmpty(str) ? TypeCache.GetType(str) : null; }

        private TypeConverter() { }
    }
}