using NUnit.Framework;

namespace ShuHai
{
    public class StringExtensionsTests
    {
        [Test]
        public void RemoveHead()
        {
            Assert.AreEqual("String", "22String".RemoveHead(2));
            Assert.AreEqual("", "1".RemoveHead(1));
            Assert.AreEqual("", "".RemoveHead(0));
        }
        
        [Test]
        public void RemoveTail()
        {
            Assert.AreEqual("String", "String223".RemoveTail(3));
            Assert.AreEqual("", "1".RemoveTail(1));
            Assert.AreEqual("", "".RemoveTail(0));
        }
    }
}