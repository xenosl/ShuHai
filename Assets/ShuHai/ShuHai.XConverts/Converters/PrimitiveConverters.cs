using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    public abstract class PrimitiveConverter : ValueConverter
    {
//        public const string ValueStyleAttributeName = "ValueStyle";

//        protected override void PopulateXAttributes(XElement element, object @object, XConvertSettings settings)
//        {
//            base.PopulateXAttributes(element, @object, settings);
//        }

        protected override object ToObjectImpl(XElement element, XConvertSettings settings)
        {
            return Parse(element.Value, settings);
        }

        protected abstract object Parse(string value, XConvertSettings settings);

        #region Value Converts

        #region Value To String

        public static string ToString(bool value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(char value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(short value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(ushort value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(int value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(uint value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(long value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(ulong value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(float value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        public static string ToString(double value, ValueStyle style)
        {
            return ToString(value, style, BitConverterEx.ToString);
        }

        private static string ToString<T>(T value, ValueStyle style, Func<T, string> byteConvert)
        {
            switch (style)
            {
                case ValueStyle.Text:
                    return value.ToString();
                case ValueStyle.Byte:
                    return byteConvert(value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        #endregion Value To String

        #region String To Value

        public static char ToChar(string text, ValueStyle style)
        {
            return ToValue(text, style, char.Parse, BitConverter.ToChar);
        }

        public static bool ToBoolean(string text, ValueStyle style)
        {
            return ToValue(text, style, bool.Parse, BitConverter.ToBoolean);
        }

        public static short ToInt16(string text, ValueStyle style)
        {
            return ToValue(text, style, short.Parse, BitConverter.ToInt16);
        }

        public static ushort ToUInt16(string text, ValueStyle style)
        {
            return ToValue(text, style, ushort.Parse, BitConverter.ToUInt16);
        }

        public static int ToInt32(string text, ValueStyle style)
        {
            return ToValue(text, style, int.Parse, BitConverter.ToInt32);
        }

        public static uint ToUInt32(string text, ValueStyle style)
        {
            return ToValue(text, style, uint.Parse, BitConverter.ToUInt32);
        }

        public static long ToInt64(string text, ValueStyle style)
        {
            return ToValue(text, style, long.Parse, BitConverter.ToInt64);
        }

        public static ulong ToUInt64(string text, ValueStyle style)
        {
            return ToValue(text, style, ulong.Parse, BitConverter.ToUInt64);
        }

        public static float ToSingle(string text, ValueStyle style)
        {
            return ToValue(text, style, float.Parse, BitConverter.ToSingle);
        }

        public static double ToDouble(string text, ValueStyle style)
        {
            return ToValue(text, style, double.Parse, BitConverter.ToDouble);
        }

        private static T ToValue<T>(string text, ValueStyle style,
            Func<string, T> textParser, Func<byte[], int, T> byteParser)
        {
            switch (style)
            {
                case ValueStyle.Text:
                    return textParser(text);
                case ValueStyle.Byte:
                    return byteParser(BitConverterEx.FromString(text), 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        #endregion String To Value

        #endregion Value Converts
    }

    [XConvertType(typeof(bool))]
    internal sealed class BooleanConverter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return bool.Parse(value); }
    }

    [XConvertType(typeof(char))]
    internal sealed class CharConverter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return char.Parse(value); }
    }

    [XConvertType(typeof(byte))]
    internal sealed class ByteConverter : PrimitiveConverter
    {
        public static readonly ByteConverter Instance = new ByteConverter();

        protected override object Parse(string value, XConvertSettings settings) { return byte.Parse(value); }
    }

    [XConvertType(typeof(sbyte))]
    internal sealed class SByteConverter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return sbyte.Parse(value); }
    }

    [XConvertType(typeof(short))]
    internal sealed class Int16Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return short.Parse(value); }
    }

    [XConvertType(typeof(ushort))]
    internal sealed class UInt16Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return ushort.Parse(value); }
    }

    [XConvertType(typeof(int))]
    internal sealed class Int32Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return int.Parse(value); }
    }

    [XConvertType(typeof(uint))]
    internal sealed class UInt32Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return uint.Parse(value); }
    }

    [XConvertType(typeof(long))]
    internal sealed class Int64Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return long.Parse(value); }
    }

    [XConvertType(typeof(ulong))]
    internal sealed class UInt64Converter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings) { return ulong.Parse(value); }
    }

    [XConvertType(typeof(float))]
    internal sealed class SingleConverter : PrimitiveConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((float)value, settings.FloatingPointStyle);
        }

        protected override object Parse(string value, XConvertSettings settings)
        {
            return ToSingle(value, settings.FloatingPointStyle);
        }
    }

    [XConvertType(typeof(double))]
    internal sealed class DoubleConverter : PrimitiveConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((double)value, settings.FloatingPointStyle);
        }

        protected override object Parse(string value, XConvertSettings settings)
        {
            return ToDouble(value, settings.FloatingPointStyle);
        }
    }

    [XConvertType(typeof(IntPtr))]
    internal sealed class IntPtrConverter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings)
        {
            return IntPtr.Size == 4 ? (IntPtr)int.Parse(value) : (IntPtr)long.Parse(value);
        }
    }

    [XConvertType(typeof(UIntPtr))]
    internal sealed class UIntPtrConverter : PrimitiveConverter
    {
        protected override object Parse(string value, XConvertSettings settings)
        {
            return UIntPtr.Size == 4 ? (UIntPtr)uint.Parse(value) : (UIntPtr)ulong.Parse(value);
        }
    }
}