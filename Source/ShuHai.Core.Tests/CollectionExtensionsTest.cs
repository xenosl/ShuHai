using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class CollectionExtensionsTest
    {
        public const int ListElementCount = 100;

        [Test]
        public void ListForEach()
        {
            var list = new List<int>();
            for (var i = 0; i < ListElementCount; ++i)
                list.Add(i);

            {
                var indices = ListForEach(list, 0, 0);
                Assert.IsTrue(indices.Count == 1);
                Assert.AreEqual(0, indices[0]);
            }
            {
                var indices = ListForEach(list, 0, 3, 1);
                Assert.IsTrue(indices.Count == 2);
                Assert.AreEqual(0, indices[0]);
                Assert.AreEqual(1, indices[1]);
                //Assert.AreEqual(2, indices[2]);
                //Assert.AreEqual(3, indices[3]);
            }
            {
                var indices = ListForEach(list, 99, 97);
                Assert.IsTrue(indices.Count == 3);
                Assert.AreEqual(99, indices[0]);
                Assert.AreEqual(98, indices[1]);
                Assert.AreEqual(97, indices[2]);
            }
            {
                var indices = ListForEach(list, 56, 58);
                Assert.IsTrue(indices.Count == 3);
                Assert.AreEqual(56, indices[0]);
                Assert.AreEqual(57, indices[1]);
                Assert.AreEqual(58, indices[2]);
            }
        }

        private static List<int> ListForEach(IReadOnlyList<int> list,
            int startIndex, int endIndex, int breakIndex = int.MaxValue)
        {
            var accessedIndices = new List<int>();
            list.ForEach(startIndex, endIndex, (index, value) =>
            {
                accessedIndices.Add(index);
                return index != breakIndex;
            });
            return accessedIndices;
        }
    }
}