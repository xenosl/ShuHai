using NUnit.Framework;

namespace ShuHai.NOctree.Test
{
    public class NOctNodeTest
    {
        [Test]
        public void EmptyNode()
        {
            var node = new NOctNode();
            Assert.AreEqual(0, node.N);
            Assert.AreEqual(0, node.ChildCount);
            Assert.AreEqual(0, node.MaxChildCount);
            Assert.AreEqual(0, node.SegmentCount);

            Assert.AreEqual(NOctIndex.Zero, node.Index);
            Assert.IsNull(node.Parent);
            Assert.IsTrue(node.IsRoot);
            Assert.AreEqual(node, node.FindRoot());

            Assert.IsTrue(node.IsLeaf);
        }

        [Test]
        public void NNode()
        {
            NNode(1, 8, 2);
            NNode(2, 64, 4);
            NNode(3, 512, 8);
            NNode(4, 4096, 16);
        }

        private static void NNode(int n, int maxChildCount, int segmentCount)
        {
            var node = new NOctNode(n);
            Assert.AreEqual(n, node.N);
            Assert.AreEqual(0, node.ChildCount);
            Assert.AreEqual(maxChildCount, node.MaxChildCount);
            Assert.AreEqual(segmentCount, node.SegmentCount);

            Assert.AreEqual(NOctIndex.Zero, node.Index);

            Assert.IsNull(node.Parent);
            Assert.IsTrue(node.IsRoot);
            Assert.AreEqual(node, node.FindRoot());

            Assert.IsFalse(node.IsLeaf);
        }

        [Test]
        public void SetChild()
        {
            var node = new NOctNode(2);

            var child1 = new NOctNode();
            node.SetChild(1, 1, 0, child1);
            Assert.AreEqual(node, child1.Parent);
            Assert.AreEqual(child1, node[1, 1, 0]);
            Assert.AreEqual(1, node.ChildCount);
            Assert.IsTrue(child1.IsLeaf);

            var child2 = new NOctNode();
            node.SetChild(1, 1, 0, child2);
            Assert.IsNull(child1.Parent);
            Assert.AreEqual(node, child2.Parent);
            Assert.AreEqual(child2, node[1, 1, 0]);
            Assert.AreEqual(1, node.ChildCount);
            Assert.IsTrue(child2.IsLeaf);

            node.SetChild(1,1,0, null);
            Assert.AreEqual(0, node.ChildCount);
            Assert.IsNull(child2.Parent);
        }

        [Test]
        public void CreateParent()
        {
            //var node = new NOctNode();
            //var parent = node.CreateParent(2, 1, 1, 1);
            //Assert.AreEqual(node.Parent, parent);
            //Assert.IsTrue(parent.IsRoot);
            //Assert.AreEqual(parent.N, 2);
            //Assert.AreEqual(parent.MaxChildCount, 64);
            //Assert.AreEqual(parent.SegmentCount, 4);
            //Assert.AreEqual(parent[1, 1, 1], node);
        }

        [Test]
        public void NInParent()
        {
            //var node = new NOctNode(1);
            //var root = node.CreateParent(2, 0, 1, 2);
            //var leaf = node.GetOrCreateChild(0, 1, 1).Node;
            //Assert.AreEqual(3, leaf.NInParent(root));
        }
    }
}
