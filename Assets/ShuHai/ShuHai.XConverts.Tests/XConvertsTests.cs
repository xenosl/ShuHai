using System;
using System.Collections.Generic;
using NUnit.Framework;
using ShuHai.XConverts.Converters;

namespace ShuHai.XConverts
{
    public class XConvertsTests
    {
        public static XConvertSettings SettingsForByteValues { get; } = CreateSettingsForByteValues();

        private static XConvertSettings CreateSettingsForByteValues()
        {
            var converters = new XConverterCollection(XConverter.BuiltIns.Values)
            {
                { new BooleanConverter(ValueStyle.Byte), true },
                { new CharConverter(ValueStyle.Byte), true },
                { new ByteConverter(ValueStyle.Byte), true },
                { new SByteConverter(ValueStyle.Byte), true },
                { new Int16Converter(ValueStyle.Byte), true },
                { new UInt16Converter(ValueStyle.Byte), true },
                { new Int32Converter(ValueStyle.Byte), true },
                { new UInt32Converter(ValueStyle.Byte), true },
                { new Int64Converter(ValueStyle.Byte), true },
                { new UInt64Converter(ValueStyle.Byte), true },
                { new SingleConverter(ValueStyle.Byte), true },
                { new DoubleConverter(ValueStyle.Byte), true }
            };
            return new XConvertSettings { Converters = converters };
        }

        [Test]
        public void ObjectConvert()
        {
            var c = XConverter.Default;

            c.ConvertedEqual(null);
            c.ConvertedEqual(2);
            c.ConvertedEqual(4.231f);
            c.ConvertedEqual(931.5523358);
            c.ConvertedEqual(Guid.NewGuid());
            c.ConvertedEqual(new KeyValuePair<int, string>(12, "Twelve"));

            c.ConvertedEqual('c');
            c.ConvertedEqual('$');
            c.ConvertedEqual(' ');
            c.ConvertedEqual('\\');
            c.ConvertedEqual('\t');

            c.ConvertedEqual(0);
            c.ConvertedEqual(12);
            c.ConvertedEqual(-22);
            c.ConvertedEqual(int.MinValue);
            c.ConvertedEqual(int.MaxValue);

            c.ConvertedEqual(0f);
            c.ConvertedEqual(12.122f);
            c.ConvertedEqual(-22.001f);
            c.ConvertedEqual(float.NaN);
            c.ConvertedEqual(float.Epsilon);

            c.ConvertedEqual(float.MinValue, SettingsForByteValues);
            c.ConvertedEqual(float.MaxValue, SettingsForByteValues);
        }

        [Test]
        public void TypeConvert()
        {
            var c = TypeConverter.Default;
            c.ConvertedEqual(typeof(string));
            c.ConvertedEqual(typeof(XConvertsTests));
        }

        [Test]
        public void StringConvert()
        {
            var c = StringConverter.Default;
            c.ConvertedEqual(null);
            c.ConvertedEqual(string.Empty);
            c.ConvertedEqual("This is a string.");
        }

        [Test]
        public void DecimalConvert()
        {
            var c = DecimalConverter.Default;
            c.ConvertedEqual(12.23m);
            c.ConvertedEqual(decimal.MinValue);
            c.ConvertedEqual(decimal.MaxValue);
            c.ConvertedEqual(decimal.MinusOne);
            c.ConvertedEqual(decimal.Zero);
            c.ConvertedEqual(decimal.One);
        }

        [Test]
        public void CollectionConvert()
        {
            var c = CollectionConverter.Default;
            var c0 = new HashSet<string>();
            var c1 = new List<object> { 978.44, Guid.NewGuid() };
            //var c2 = new Dictionary<int, object> { { 1, "string item" }, { 2, 231 }, { 3, float.MaxValue }, { 4, c1 } };
            var c3 = new object[] { 22, "string" };
            //var c4 = new object[,] { { 33, "str", 'c' }, { 2212, 23.3f, Guid.NewGuid() } };

            c.ConvertedEqual(c0, null, SequentialEqualityComparer<string>.Default);
            c.ConvertedEqual(c1, null, SequentialEqualityComparer<object>.Default);
            //c.ConvertedEqual(c2, null, SequentialEqualityComparer<KeyValuePair<int, object>>.Default);
            c.ConvertedEqual(c3, null, SequentialEqualityComparer<object>.Default);
            //c.ConvertedEqual(c4);
        }

        [Test]
        public void GuidConvert()
        {
            var c = GuidConverter.Default;
            c.ConvertedEqual(Guid.Empty);
            c.ConvertedEqual(Guid.NewGuid());
            c.ConvertedEqual(Guid.NewGuid());
        }

        [Test]
        public void DateTimeConvert()
        {
            var c = DateTimeConverter.Default;
            c.ConvertedEqual(new DateTime());
            c.ConvertedEqual(DateTime.Now);
            c.ConvertedEqual(DateTime.UtcNow);
            c.ConvertedEqual(DateTime.Today);
            c.ConvertedEqual(DateTime.MinValue);
            c.ConvertedEqual(DateTime.MaxValue);
        }
    }
}