using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.Unity
{
    public static class AssertEx
    {
        public static void AreEqual(object expected, object actual, IEqualityComparer comparer)
        {
            comparer = comparer ?? EqualityComparer<object>.Default;
            Assert.IsTrue(comparer.Equals(expected, actual));
        }

        public static void AreEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            Assert.IsTrue(comparer.Equals(expected, actual));
        }
    }
}