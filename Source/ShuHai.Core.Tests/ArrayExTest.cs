using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class ArrayExTest
    {
        [TestCase(new[] { 2, 3 }, new[] { 3, 4 }, new[] { 2, 3, 3, 4 })]
        [TestCase(new int[0], new[] { 3, 4 }, new[] { 3, 4 })]
        [TestCase(new int[0], new int[0], new int[0])]
        public void Added(int[] self, IEnumerable<int> values, int[] result)
        {
            var added = self.Added(values);
            CollectionAssert.AreEqual(result, added);
        }
    }
}
