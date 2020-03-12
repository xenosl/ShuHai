using System;
using System.Globalization;
using NUnit.Framework;

namespace ShuHai.SConverts.Converters
{
    public class PrimitiveConvertersTests : SConverterTests
    {
        [Test]
        public void CharConvert()
        {
            var c = CharConverter.Instance;
            Convert(c, 'A', "A");
            Convert(c, ' ', " ");
        }

        [Test]
        public void BooleanConvert()
        {
            var c = BooleanConverter.Instance;
            Convert(c, true, "True");
            Convert(c, false, "False");
        }

        [Test]
        public void SByteConvert()
        {
            var c = SByteConverter.Instance;
            Convert(c, (SByte)22, "22");
            Convert(c, (SByte)0, "0");
            Convert(c, SByte.MaxValue, SByte.MaxValue.ToString());
        }

        [Test]
        public void Int32Convert()
        {
            var c = Int32Converter.Instance;
            Convert(c, 32, "32");
            Convert(c, -12331, "-12331");
            Convert(c, 0, "0");
            Convert(c, Int32.MinValue, Int32.MinValue.ToString());
        }

        [Test]
        public void SingleConvert()
        {
            var c = SingleConverter.Instance;
            Convert(c, 21.22f, "21.22");
            Convert(c, 0f, "0");
            Convert(c, Single.NaN, Single.NaN.ToString(CultureInfo.InvariantCulture));
        }
    }
}