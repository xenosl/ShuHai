using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ShuHai.Edit.ObjectDataConverters;

namespace ShuHai.Edit.Tests
{
    public class CollectionConverterTests
    {
        [Test]
        public void SimpleConvert()
        {
            ConvertTest(new object[] { });
            ConvertTest(new object[] { "string item" });
            ConvertTest(new object[] { "string item", typeof(CollectionConverterTests), 2, 23.22 });

            ConvertTest(new List<object>());
            ConvertTest(new HashSet<object> { "string item" });
            ConvertTest(new List<object> { "string item", typeof(CollectionConverterTests), 2, 23.22 });
        }

        public static void ConvertTest(IEnumerable input)
        {
            var c = CollectionConverter.Default;

            var data = c.ToData(input);
            Assert.AreEqual(input.GetType(), data.ObjectType);

            var obj = (IEnumerable)c.ToObject(data);
            Assert.IsTrue(Utilities.CollectionEquals(input, obj));
        }
    }
}