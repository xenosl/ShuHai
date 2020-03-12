using NUnit.Framework;
using UnityEngine;

namespace ShuHai.Unity
{
    public class TransformExtensionsTests
    {
        [Test]
        public void MakeHierarchicName()
        {
            var root = new GameObject("Root").transform;
            var c1 = new GameObject("1").transform;
            c1.parent = root;
            var c2 = new GameObject("2").transform;
            c2.parent = c1;

            Assert.AreEqual(root.name, root.MakeHierarchicName());
            Assert.AreEqual("Root/1", c1.MakeHierarchicName());
            Assert.AreEqual("Root/1/2", c2.MakeHierarchicName());
        }
    }
}