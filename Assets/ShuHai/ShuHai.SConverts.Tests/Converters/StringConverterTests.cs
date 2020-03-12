using NUnit.Framework;

namespace ShuHai.SConverts.Converters
{
    public class StringConverterTests : SConverterTests
    {
        [Test]
        public void CanConvert()
        {
            var c = StringConverter.Instance;

            Assert.IsTrue(c.CanConvert("Hello, String!"));
            Assert.IsFalse(c.CanConvert(32));

            Assert.IsTrue(c.CanConvert(typeof(string)));
            Assert.IsFalse(c.CanConvert(typeof(object)));
            Assert.IsFalse(c.CanConvert(typeof(int)));
        }

        [Test]
        public void Convert()
        {
            Convert("Hello, String!", "Hello, String!");
            Convert("", "");
        }

        private static void Convert(object val, string str) { Convert(StringConverter.Instance, val, str); }
    }
}