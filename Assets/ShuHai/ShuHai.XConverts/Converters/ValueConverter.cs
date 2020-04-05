using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    /// <summary>
    ///     Abstract converter for all types that doesn't need any child xml element.
    /// </summary>
    public abstract class ValueConverter : XConverter
    {
        public ValueStyle ValueStyle { get; set; }

        protected ValueConverter() { }
        protected ValueConverter(ValueStyle valueStyle) { ValueStyle = valueStyle; }

        #region Value To XElement

        protected virtual string ValueToString(object value, XConvertSettings settings) { return value.ToString(); }

        protected sealed override void PopulateXElementValue(
            XElement element, object @object, XConvertSettings settings)
        {
            element.Value = ValueToString(@object, settings);
        }

        protected sealed override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion Value To XElement

        #region XElement To Value

        protected sealed override void PopulateObjectMembers(
            object @object, XElement element, XConvertSettings settings)
        {
            // Nothing to do...
        }

        #endregion XElement To Value

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

        public static sbyte ToSByte(string text, ValueStyle style)
        {
            return ToValue(text, style, sbyte.Parse,
                (value, startIndex) => (sbyte)BitConverter.ToInt16(value, startIndex));
        }

        public static byte ToByte(string text, ValueStyle style)
        {
            return ToValue(text, style, byte.Parse,
                (value, startIndex) => (byte)BitConverter.ToUInt16(value, startIndex));
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
}