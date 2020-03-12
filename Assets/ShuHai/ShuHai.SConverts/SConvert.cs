using System;

namespace ShuHai.SConverts
{
    public static class SConvert
    {
        public static string ToString(object value) { return ToString(value, SConvertToStringOptions.Default); }

        public static string ToString(object value, SConvertToStringOptions options)
        {
            Ensure.Argument.NotNull(options, nameof(options));

            return value != null
                ? GetConverter(value.GetType(), options).ToString(value)
                : options.StringForNullValue;
        }

        public static object ToValue(Type targetType, string str)
        {
            return ToValue(targetType, str, SConvertToValueOptions.Default);
        }

        public static object ToValue(Type targetType, string str, SConvertToValueOptions options)
        {
            Ensure.Argument.NotNull(options, nameof(options));

            return targetType != null
                ? GetConverter(targetType, options).ToValue(str)
                : options.ObjectForNullType;
        }

        private static ISConverter GetConverter(Type targetType, SConvertOptions options)
        {
            var converters = options.Converters ?? SConverterCollection.Default;
            var converter = converters.GetByTargetType(targetType);
            if (converter == null)
                throw new NotSupportedException($@"String converter for ""{targetType}"" not found.");
            return converter;
        }
    }
}