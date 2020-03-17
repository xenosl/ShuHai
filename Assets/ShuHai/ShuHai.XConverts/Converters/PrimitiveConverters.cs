using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    internal abstract class PrimitiveConverter : XConverter
    {
        #region Object To XElement

        protected sealed override void PopulateXElementValue(XElement element, object @object,
            XConvertSettings settings)
        {
            element.Value = @object.ToString();
        }

        protected sealed override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion Object To XElement

        #region XElement To Object

        protected override object ToObjectImpl(XElement element, XConvertSettings settings)
        {
            return Parse(element.Value);
        }

        protected abstract object Parse(string value);

        protected sealed override void PopulateObjectMembersImpl(
            object @object, XElement element, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion XElement To Object
    }

    [XConvertType(typeof(bool))]
    internal sealed class BooleanConverter : PrimitiveConverter
    {
        protected override object Parse(string value) { return bool.Parse(value); }
    }

    [XConvertType(typeof(char))]
    internal sealed class CharConverter : PrimitiveConverter
    {
        protected override object Parse(string value) { return char.Parse(value); }
    }

    [XConvertType(typeof(byte))]
    internal sealed class ByteConverter : PrimitiveConverter
    {
        public static readonly ByteConverter Instance = new ByteConverter();

        protected override object Parse(string value) { return byte.Parse(value); }
    }

    [XConvertType(typeof(sbyte))]
    internal sealed class SByteConverter : PrimitiveConverter
    {
        protected override object Parse(string value) { return sbyte.Parse(value); }
    }

    [XConvertType(typeof(short))]
    internal sealed class Int16Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return short.Parse(value); }
    }

    [XConvertType(typeof(ushort))]
    internal sealed class UInt16Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return ushort.Parse(value); }
    }

    [XConvertType(typeof(int))]
    internal sealed class Int32Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return int.Parse(value); }
    }

    [XConvertType(typeof(uint))]
    internal sealed class UInt32Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return uint.Parse(value); }
    }

    [XConvertType(typeof(long))]
    internal sealed class Int64Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return long.Parse(value); }
    }

    [XConvertType(typeof(ulong))]
    internal sealed class UInt64Converter : PrimitiveConverter
    {
        protected override object Parse(string value) { return ulong.Parse(value); }
    }

    [XConvertType(typeof(float))]
    internal sealed class SingleConverter : PrimitiveConverter
    {
        // TODO: Use BitConverter to ensure precision.
        protected override object Parse(string value) { return float.Parse(value); }
    }

    [XConvertType(typeof(double))]
    internal sealed class DoubleConverter : PrimitiveConverter
    {
        // TODO: Use BitConverter to ensure precision.
        protected override object Parse(string value) { return double.Parse(value); }
    }

    [XConvertType(typeof(IntPtr))]
    internal sealed class IntPtrConverter : PrimitiveConverter
    {
        protected override object Parse(string value)
        {
            return IntPtr.Size == 4 ? (IntPtr)int.Parse(value) : (IntPtr)long.Parse(value);
        }
    }

    [XConvertType(typeof(UIntPtr))]
    internal sealed class UIntPtrConverter : PrimitiveConverter
    {
        protected override object Parse(string value)
        {
            return UIntPtr.Size == 4 ? (UIntPtr)uint.Parse(value) : (UIntPtr)ulong.Parse(value);
        }
    }
}