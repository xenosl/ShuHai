using System;
using NUnit.Framework;

namespace ShuHai.SConverts.Converters
{
    public class SConverterCollectionTests
    {
        [SConverter(1)]
        private sealed class CustomInt32Converter : SConverter<Int32>
        {
            public override int ToValue(string str) { throw new NotImplementedException(); }
        }

        [Test]
        public void Modify()
        {
            var cc = new SConverterCollection();

            Assert.AreEqual(0, cc.Count);
            CollectionAssert.AreEquivalent(Array.Empty<SConverter>(), cc);

            Assert.IsTrue(cc.Add(StringConverter.Instance));
            Assert.AreEqual(1, cc.Count);
            CollectionAssert.AreEquivalent(cc, new[] { StringConverter.Instance });

            Assert.IsTrue(cc.Add(Int32Converter.Instance));
            Assert.AreEqual(2, cc.Count);
            CollectionAssert.AreEquivalent(cc, new SConverter[] { StringConverter.Instance, Int32Converter.Instance });

            Assert.IsFalse(cc.Add(Int32Converter.Instance));
            Assert.AreEqual(2, cc.Count);
            CollectionAssert.AreEquivalent(cc, new SConverter[] { StringConverter.Instance, Int32Converter.Instance });

            var myInt32Converter = new CustomInt32Converter();
            Assert.IsTrue(cc.Add(myInt32Converter));
            Assert.AreEqual(3, cc.Count);
            CollectionAssert.AreEquivalent(cc,
                new SConverter[] { StringConverter.Instance, Int32Converter.Instance, myInt32Converter });

            Assert.IsFalse(cc.Remove(BooleanConverter.Instance));

            Assert.IsTrue(cc.Remove(StringConverter.Instance));
            Assert.AreEqual(2, cc.Count);
            CollectionAssert.AreEquivalent(cc, new SConverter[] { Int32Converter.Instance, myInt32Converter });

            cc.Clear();
            Assert.AreEqual(0, cc.Count);
            CollectionAssert.AreEquivalent(cc, Array.Empty<SConverter>());
        }

        [Test]
        public void Priority()
        {
            var cc = new SConverterCollection();
            var customConverter = new CustomInt32Converter();

            cc.Add(customConverter);
            cc.Add(Int32Converter.Instance);
            Assert.AreEqual(customConverter, cc.GetByTargetType(typeof(Int32)));

            cc.Clear();

            cc.Add(Int32Converter.Instance);
            cc.Add(customConverter);
            Assert.AreEqual(customConverter, cc.GetByTargetType(typeof(Int32)));
        }
    }
}