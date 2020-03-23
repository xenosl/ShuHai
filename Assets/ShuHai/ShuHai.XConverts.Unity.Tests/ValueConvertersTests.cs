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

            XConvertsTests.ConvertTest(c, Vector2.zero);
            XConvertsTests.ConvertTest(c, Vector2.one);
            XConvertsTests.ConvertTest(c, new Vector2(0.2f, -951.1782f));
            //XConvertsTests.ConvertTest(c, new Vector2(0.2f, float.MinValue), bytesForFp);
        }

        [Test]
        public void Vector3Convert()
        {
            var c = new Vector3Converter();

            XConvertsTests.ConvertTest(c, Vector3.zero);
            XConvertsTests.ConvertTest(c, Vector3.one);
            XConvertsTests.ConvertTest(c, new Vector3(0.2f, -951.1782f, 22f));
            //XConvertsTests.ConvertTest(c, new Vector3(0.2f, 22f, -231.1786522122223098234975622f), bytesForFp);
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
        public void Vector2IntConvert()
        {
            var c = new Vector2IntConverter();
            XConvertsTests.ConvertTest(c, Vector2Int.zero);
            XConvertsTests.ConvertTest(c, Vector2Int.one);
            XConvertsTests.ConvertTest(c, new Vector2Int(int.MinValue, int.MaxValue));
        }

        [Test]
        public void Vector3IntConvert()
        {
            var c = new Vector3IntConverter();
            XConvertsTests.ConvertTest(c, Vector3Int.zero);
            XConvertsTests.ConvertTest(c, Vector3Int.one);
            XConvertsTests.ConvertTest(c, new Vector3Int(int.MinValue, int.MaxValue, 0));
        }

        [Test]
        public void QuaternionConvert()
        {
            var c = new QuaternionConverter();

            XConvertsTests.ConvertTest(c, Quaternion.identity);
            //XConvertsTests.ConvertTest(c, Quaternion.AngleAxis(0.2577662f, Vector3.back), bytesForFp);
            //XConvertsTests.ConvertTest(c, new Quaternion(0.55477f, 0.77f, float.MinValue, 1), bytesForFp);
        }

        [Test]
        public void RectConvert()
        {
            var c = new RectConverter();
            XConvertsTests.ConvertTest(c, Rect.zero);
            XConvertsTests.ConvertTest(c, new Rect(0f, 22.43f, 280f, 2244.11f));
        }
    }
}