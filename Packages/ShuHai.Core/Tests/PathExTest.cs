using System;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class PathExTest
    {
        [Test]
        public void DirectorySeparator()
        {
            PathEx.DirectorySeparatorStyle = DirectorySeparatorStyle.Backslash;
            Assert.AreEqual('\\', PathEx.DirectorySeparator);
            Assert.AreEqual('/', PathEx.AltDirectorySeparator);

            PathEx.DirectorySeparatorStyle = DirectorySeparatorStyle.Slash;
            Assert.AreEqual('/', PathEx.DirectorySeparator);
            Assert.AreEqual('\\', PathEx.AltDirectorySeparator);
        }

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
            CollectionAssert.AreEqual(names, PathEx.Split(path));
        }
    }
}