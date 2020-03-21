using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    public enum ValueStyle { Text, Byte }

    public abstract class PrimitiveConverter : ValueConverter
    {
//        public const string ValueStyleAttributeName = "ValueStyle";

//        protected override void PopulateXAttributes(XElement element, object @object, XConvertSettings settings)
//        {
//            base.PopulateXAttributes(element, @object, settings);
//        }

        public ValueStyle ValueStyle { get; set; }

        protected PrimitiveConverter() { }

        protected PrimitiveConverter(ValueStyle valueStyle) { ValueStyle = valueStyle; }

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
    public sealed class BooleanConverter : PrimitiveConverter
    {
        public BooleanConverter() { }
        public BooleanConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return bool.Parse(value); }
    }

    [XConvertType(typeof(char))]
    public sealed class CharConverter : PrimitiveConverter
    {
        public CharConverter() { }
        public CharConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return char.Parse(value); }
    }

    [XConvertType(typeof(byte))]
    public sealed class ByteConverter : PrimitiveConverter
    {
        public ByteConverter() { }
        public ByteConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return byte.Parse(value); }
    }

    [XConvertType(typeof(sbyte))]
    public sealed class SByteConverter : PrimitiveConverter
    {
        public SByteConverter() { }
        public SByteConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return sbyte.Parse(value); }
    }

    [XConvertType(typeof(short))]
    public sealed class Int16Converter : PrimitiveConverter
    {
        public Int16Converter() { }
        public Int16Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return short.Parse(value); }
    }

    [XConvertType(typeof(ushort))]
    public sealed class UInt16Converter : PrimitiveConverter
    {
        public UInt16Converter() { }
        public UInt16Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return ushort.Parse(value); }
    }

    [XConvertType(typeof(int))]
    public sealed class Int32Converter : PrimitiveConverter
    {
        public Int32Converter() { }
        public Int32Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return int.Parse(value); }
    }

    [XConvertType(typeof(uint))]
    public sealed class UInt32Converter : PrimitiveConverter
    {
        public UInt32Converter() { }
        public UInt32Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return uint.Parse(value); }
    }

    [XConvertType(typeof(long))]
    public sealed class Int64Converter : PrimitiveConverter
    {
        public Int64Converter() { }
        public Int64Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return long.Parse(value); }
    }

    [XConvertType(typeof(ulong))]
    public sealed class UInt64Converter : PrimitiveConverter
    {
        public UInt64Converter() { }
        public UInt64Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings) { return ulong.Parse(value); }
    }

    [XConvertType(typeof(float))]
    public sealed class SingleConverter : PrimitiveConverter
    {
        public SingleConverter() { }
        public SingleConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((float)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToSingle(value, ValueStyle); }
    }

    [XConvertType(typeof(double))]
    public sealed class DoubleConverter : PrimitiveConverter
    {
        public DoubleConverter() { }
        public DoubleConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((double)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToDouble(value, ValueStyle); }
    }

    [XConvertType(typeof(IntPtr))]
    public sealed class IntPtrConverter : PrimitiveConverter
    {
        public IntPtrConverter() { }
        public IntPtrConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings)
        {
            return IntPtr.Size == 4 ? (IntPtr)int.Parse(value) : (IntPtr)long.Parse(value);
        }
    }

    [XConvertType(typeof(UIntPtr))]
    public sealed class UIntPtrConverter : PrimitiveConverter
    {
        public UIntPtrConverter() { }
        public UIntPtrConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object Parse(string value, XConvertSettings settings)
        {
            return UIntPtr.Size == 4 ? (UIntPtr)uint.Parse(value) : (UIntPtr)ulong.Parse(value);
        }
    }
}