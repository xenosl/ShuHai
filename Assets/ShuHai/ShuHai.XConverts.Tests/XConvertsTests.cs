using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.XConverts
{
    public class XConvertsTests
    {
        [Test]
        public void ObjectConvert()
        {
            var c = XConverter.Default;

            ConvertTest(c, null);
            ConvertTest(c, 2);
            ConvertTest(c, 4.231f);
            ConvertTest(c, 931.5523358);
            ConvertTest(c, Guid.NewGuid());

            ConvertTest(c, 'c');
            ConvertTest(c, '$');
            ConvertTest(c, ' ');
            ConvertTest(c, '\\');
            ConvertTest(c, '\t');

            ConvertTest(c, 0);
            ConvertTest(c, 12);
            ConvertTest(c, -22);
            ConvertTest(c, int.MinValue);
            ConvertTest(c, int.MaxValue);

            ConvertTest(c, 0f);
            ConvertTest(c, 12.122f);
            ConvertTest(c, -22.001f);
            ConvertTest(c, float.NaN);
            ConvertTest(c, float.Epsilon);

            var bytesForFp = new XConvertSettings { FloatingPointStyle = ValueStyle.Byte };
            ConvertTest(c, float.MinValue, bytesForFp);
            ConvertTest(c, float.MaxValue, bytesForFp);
        }

        [Test]
        public void StringConvert()
        {
            var c = XConverter.BuiltIns[typeof(string)];
            ConvertTest(c, null);
            ConvertTest(c, string.Empty);
            ConvertTest(c, "This is a string.");
        }

        [Test]
        public void CollectionConvert()
        {
            var c = XConverter.BuiltIns[typeof(ICollection<>)];
            var bytesForFp = new XConvertSettings { FloatingPointStyle = ValueStyle.Byte };
            
            var c0 = new HashSet<string>();
            var c1 = new List<object> { 978.44, Guid.NewGuid() };
            var c2 = new Dictionary<int, object> { { 1, "string item" }, { 2, 231 }, { 3, float.MaxValue }, { 4, c1 } };
            
            ConvertTest(c, c0);
            ConvertTest(c, c1);
            ConvertTest(c, c2, bytesForFp);
        }

        [Test]
        public void GuidConvert()
        {
            var c = XConverter.BuiltIns[typeof(Guid)];
            ConvertTest(c, Guid.Empty);
            ConvertTest(c, Guid.NewGuid());
            ConvertTest(c, Guid.NewGuid());
        }

        public static void ConvertTest(XConverter converter, object value, XConvertSettings settings = null)
        {
            if (settings == null)
                settings = XConvertSettings.Default;

            var element = converter.ToXElement(value, "ConvertTest", settings);
            var obj = converter.ToObject(element, settings);
            Assert.AreEqual(value, obj);
        }
    }
}