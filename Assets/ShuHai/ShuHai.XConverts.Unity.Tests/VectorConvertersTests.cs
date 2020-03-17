using NUnit.Framework;
using UnityEngine;

namespace ShuHai.XConverts.Unity.Tests
{
    public class VectorConvertersTests
    {
        [Test]
        public void Vector2Convert()
        {
            var c = new Vector2Converter();
            XConvertsTests.ConvertTest(c, Vector2.zero);
            XConvertsTests.ConvertTest(c, Vector2.one);
            XConvertsTests.ConvertTest(c, new Vector2(0.2f, -951.1782f));
            //XConvertsTests.ConvertTest(c, new Vector3(0.2f, -231.1786522f));
        }

        [Test]
        public void Vector3Convert()
        {
            var c = new Vector3Converter();
            XConvertsTests.ConvertTest(c, Vector3.zero);
            XConvertsTests.ConvertTest(c, Vector3.one);
            XConvertsTests.ConvertTest(c, new Vector3(0.2f, -951.1782f, 22f));
            //XConvertsTests.ConvertTest(c, new Vector3(0.2f, 22f, -231.1786522f));
        }

        [Test]
        public void Vector4Convert()
        {
            var c = new Vector4Converter();
            XConvertsTests.ConvertTest(c, Vector4.zero);
            XConvertsTests.ConvertTest(c, Vector4.one);
            XConvertsTests.ConvertTest(c, new Vector4(0.2f, -951.1782f, 22f, 1.12f));
        }
    }
}