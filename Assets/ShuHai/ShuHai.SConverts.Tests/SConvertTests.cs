using System;
using NUnit.Framework;

namespace ShuHai.SConverts
{
    public class SConvertTests
    {
        [Test]
        public new void ToString()
        {
            Assert.AreEqual(string.Empty, SConvert.ToString(null));
            Assert.AreEqual("ToString Test.", SConvert.ToString("ToString Test."));
            Assert.AreEqual("23.211", SConvert.ToString(23.211));
        }

        [Test]
        public void ToValue()
        {
            Assert.AreEqual(null, SConvert.ToValue(null, null));
            Assert.AreEqual(null, SConvert.ToValue(typeof(Type), string.Empty));
            Assert.AreEqual(32, SConvert.ToValue(typeof(int), "32"));
            Assert.AreEqual("ToValue Test.", SConvert.ToValue(typeof(string), "ToValue Test."));
        }
    }
}