using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    public abstract class PrimitiveConverter : ValueConverter
    {
        protected PrimitiveConverter() { }
        protected PrimitiveConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return Parse(element.Value, settings);
        }

        protected abstract object Parse(string value, XConvertSettings settings);
    }

    [XConvertType(typeof(bool))]
    public sealed class BooleanConverter : PrimitiveConverter
    {
        public BooleanConverter() { }
        public BooleanConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((bool)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings)
        {
            return ToBoolean(value, ValueStyle);
        }
    }

    [XConvertType(typeof(char))]
    public sealed class CharConverter : PrimitiveConverter
    {
        public CharConverter() { }
        public CharConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((char)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToChar(value, ValueStyle); }
    }

    [XConvertType(typeof(byte))]
    public sealed class ByteConverter : PrimitiveConverter
    {
        public ByteConverter() { }
        public ByteConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((byte)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToByte(value, ValueStyle); }
    }

    [XConvertType(typeof(sbyte))]
    public sealed class SByteConverter : PrimitiveConverter
    {
        public SByteConverter() { }
        public SByteConverter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((sbyte)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToSByte(value, ValueStyle); }
    }

    [XConvertType(typeof(short))]
    public sealed class Int16Converter : PrimitiveConverter
    {
        public Int16Converter() { }
        public Int16Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((short)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToInt16(value, ValueStyle); }
    }

    [XConvertType(typeof(ushort))]
    public sealed class UInt16Converter : PrimitiveConverter
    {
        public UInt16Converter() { }
        public UInt16Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((ushort)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToUInt16(value, ValueStyle); }
    }

    [XConvertType(typeof(int))]
    public sealed class Int32Converter : PrimitiveConverter
    {
        public Int32Converter() { }
        public Int32Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((int)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToInt32(value, ValueStyle); }
    }

    [XConvertType(typeof(uint))]
    public sealed class UInt32Converter : PrimitiveConverter
    {
        public UInt32Converter() { }
        public UInt32Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((uint)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToUInt32(value, ValueStyle); }
    }

    [XConvertType(typeof(long))]
    public sealed class Int64Converter : PrimitiveConverter
    {
        public Int64Converter() { }
        public Int64Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((long)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToInt64(value, ValueStyle); }
    }

    [XConvertType(typeof(ulong))]
    public sealed class UInt64Converter : PrimitiveConverter
    {
        public UInt64Converter() { }
        public UInt64Converter(ValueStyle valueStyle) : base(valueStyle) { }

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            return ToString((ulong)value, ValueStyle);
        }

        protected override object Parse(string value, XConvertSettings settings) { return ToUInt64(value, ValueStyle); }
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