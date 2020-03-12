using NUnit.Framework;

namespace ShuHai.SConverts
{
    public class SConverterTests
    {
        protected static void Convert(SConverter converter, object val, string str)
        {
            string s = converter.ToString(val);
            var v = converter.ToValue(str);
            Assert.AreEqual(val, v);

            var s2v = converter.ToValue(s);
            string v2s = converter.ToString(v);
            Assert.AreEqual(val, s2v);
            Assert.AreEqual(v2s, s);
        }
    }
}