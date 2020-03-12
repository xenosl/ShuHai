using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.SConverts.Converters
{
    public class TypeConverterTests : SConverterTests
    {
        [Test]
        public void Convert()
        {
            Convert<string>();
            Convert<int>();
            Convert<TypeConverterTests>();
            Convert<IList<SConverter>>();
        }

        private static void Convert<T>()
        {
            var c = TypeConverter.Instance;
            var t = typeof(T);
            Convert(c, t, t.FullName);
        }
    }
}