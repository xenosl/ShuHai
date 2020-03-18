using NUnit.Framework;
using UnityEngine;

namespace ShuHai.XConverts.Unity.Tests
{
    public class ValueConvertersTests
    {
        [Test]
        public void Vector2Convert()
        {
            var c = new Vector2Converter();
            var bytesForFp = new XConvertSettings { FloatingPointStyle = ValueStyle.Byte };

            XConvertsTests.ConvertTest(c, Vector2.zero);
            XConvertsTests.ConvertTest(c, Vector2.one);
            XConvertsTests.ConvertTest(c, new Vector2(0.2f, -951.1782f));
            XConvertsTests.ConvertTest(c, new Vector2(0.2f, float.MinValue), bytesForFp);
        }

        [Test]
        public void Vector3Convert()
        {
            var c = new Vector3Converter();
            var bytesForFp = new XConvertSettings { FloatingPointStyle = ValueStyle.Byte };

            XConvertsTests.ConvertTest(c, Vector3.zero);
            XConvertsTests.ConvertTest(c, Vector3.one);
            XConvertsTests.ConvertTest(c, new Vector3(0.2f, -951.1782f, 22f));
            XConvertsTests.ConvertTest(c, new Vector3(0.2f, 22f, -231.1786522122223098234975622f), bytesForFp);
        }

        [Test]
        public void Vector4Convert()
        {
            var c = new Vector4Converter();
            XConvertsTests.ConvertTest(c, Vector4.zero);
            XConvertsTests.ConvertTest(c, Vector4.one);
            XConvertsTests.ConvertTest(c, new Vector4(0.2f, -951.1782f, 22f, 1.12f));
        }

        [Test]
        public void QuaternionConvert()
        {
            var c = new QuaternionConverter();
            var bytesForFp = new XConvertSettings { FloatingPointStyle = ValueStyle.Byte };

            XConvertsTests.ConvertTest(c, Quaternion.identity);
            XConvertsTests.ConvertTest(c, Quaternion.AngleAxis(0.2577662f, Vector3.back), bytesForFp);
            XConvertsTests.ConvertTest(c, new Quaternion(0.55477f, 0.77f, float.MinValue, 1), bytesForFp);
        }
    }
}