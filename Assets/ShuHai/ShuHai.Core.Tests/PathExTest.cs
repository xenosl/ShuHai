using System;
using System.Linq;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class PathExTest
    {
        [Test]
        public void Split()
        {
            Split("1", "1");
            Split("1/2/3", "1", "2", "3");
            Split("1\\/2\\3", "1", "2", "3");
            Split("D:/1\\/2\\3", "D:", "1", "2", "3");
            Split(string.Empty, Array.Empty<string>());
        }

        private static void Split(string path, params string[] names)
        {
            Assert.IsTrue(PathEx.Split(path).SequenceEqual(names));
        }
    }
}