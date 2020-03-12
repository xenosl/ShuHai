using System;

namespace ShuHai.SConverts.Converters
{
    public abstract class PrimitiveConverter<T> : SConverter<T>
        where T : struct
    {
        public override string ToString(T value) { return value.ToString(); }

        protected PrimitiveConverter()
        {
            if (!TargetType.IsPrimitive)
                throw new ArgumentException($"Only primitive type is allowed for {GetType().FullName}.");
        }
    }

    public sealed class CharConverter : PrimitiveConverter<Char>
    {
        public static readonly CharConverter Instance = new CharConverter();

        public override Char ToValue(string str) { return Char.Parse(str); }

        private CharConverter() { }
    }

    public sealed class BooleanConverter : PrimitiveConverter<Boolean>
    {
        public static readonly BooleanConverter Instance = new BooleanConverter();

        public override Boolean ToValue(string str) { return Boolean.Parse(str); }

        private BooleanConverter() { }
    }

    public sealed class SByteConverter : PrimitiveConverter<SByte>
    {
        public static readonly SByteConverter Instance = new SByteConverter();

        public override SByte ToValue(string str) { return SByte.Parse(str); }

        private SByteConverter() { }
    }

    public sealed class ByteConverter : PrimitiveConverter<Byte>
    {
        public static readonly ByteConverter Instance = new ByteConverter();

        public override Byte ToValue(string str) { return Byte.Parse(str); }

        private ByteConverter() { }
    }

    public sealed class Int16Converter : PrimitiveConverter<Int16>
    {
        public static readonly Int16Converter Instance = new Int16Converter();

        public override Int16 ToValue(string str) { return Int16.Parse(str); }

        private Int16Converter() { }
    }

    public sealed class UInt16Converter : PrimitiveConverter<UInt16>
    {
        public static readonly UInt16Converter Instance = new UInt16Converter();

        public override UInt16 ToValue(string str) { return UInt16.Parse(str); }

        private UInt16Converter() { }
    }

    public sealed class Int32Converter : PrimitiveConverter<Int32>
    {
        public static readonly Int32Converter Instance = new Int32Converter();

        public override Int32 ToValue(string str) { return Int32.Parse(str); }

        private Int32Converter() { }
    }

    public sealed class UInt32Converter : PrimitiveConverter<UInt32>
    {
        public static readonly UInt32Converter Instance = new UInt32Converter();

        public override UInt32 ToValue(string str) { return UInt32.Parse(str); }

        private UInt32Converter() { }
    }

    public sealed class Int64Converter : PrimitiveConverter<Int64>
    {
        public static readonly Int64Converter Instance = new Int64Converter();

        public override Int64 ToValue(string str) { return Int64.Parse(str); }

        private Int64Converter() { }
    }

    public sealed class UInt64Converter : PrimitiveConverter<UInt64>
    {
        public static readonly UInt64Converter Instance = new UInt64Converter();

        public override UInt64 ToValue(string str) { return UInt64.Parse(str); }

        private UInt64Converter() { }
    }

    public sealed class SingleConverter : PrimitiveConverter<Single>
    {
        public static readonly SingleConverter Instance = new SingleConverter();

        public override Single ToValue(string str) { return Single.Parse(str); }

        private SingleConverter() { }
    }

    public sealed class DoubleConverter : PrimitiveConverter<Double>
    {
        public static readonly DoubleConverter Instance = new DoubleConverter();

        public override Double ToValue(string str) { return Double.Parse(str); }

        private DoubleConverter() { }
    }

    public sealed class IntPtrConverter : PrimitiveConverter<IntPtr>
    {
        public static readonly IntPtrConverter Instance = new IntPtrConverter();

        public override IntPtr ToValue(string str)
        {
            return IntPtr.Size == 4 ? (IntPtr)Int32.Parse(str) : (IntPtr)Int64.Parse(str);
        }

        private IntPtrConverter() { }
    }

    public sealed class UIntPtrConverter : PrimitiveConverter<UIntPtr>
    {
        public static readonly UIntPtrConverter Instance = new UIntPtrConverter();

        public override UIntPtr ToValue(string str)
        {
            return UIntPtr.Size == 4 ? (UIntPtr)UInt32.Parse(str) : (UIntPtr)UInt64.Parse(str);
        }

        private UIntPtrConverter() { }
    }
}