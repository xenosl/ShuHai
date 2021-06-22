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
            Assert.AreEqual(0, node.ChildCapacity);
            Assert.AreEqual(0, node.DimensionalChildCapacity);

            Assert.AreEqual(NOctIndex.Invalid, node.Index);
            Assert.IsNull(node.Parent);
            Assert.IsTrue(node.IsRoot);
            Assert.AreEqual(node, node.FindRoot());

            Assert.IsTrue(node.IsLeaf);
        }

        [Test]
        public void NRankNode()
        {
            NRankNode(1, 8, 2);
            NRankNode(2, 64, 4);
            NRankNode(3, 512, 8);
            NRankNode(4, 4096, 16);
        }
        
        private static void NRankNode(int n, int childCapacity, int dimensionalChildCapacity)
        {
            var node = new NOctNode(n);
            Assert.AreEqual(n, node.N);
            Assert.AreEqual(0, node.ChildCount);
            Assert.AreEqual(childCapacity, node.ChildCapacity);
            Assert.AreEqual(dimensionalChildCapacity, node.DimensionalChildCapacity);

            Assert.AreEqual(NOctIndex.Invalid, node.Index);

            Assert.IsNull(node.Parent);
            Assert.IsTrue(node.IsRoot);
            Assert.AreEqual(node, node.FindRoot());

            Assert.IsFalse(node.IsLeaf);
        }

        [Test]
        public void SetChild()
        {
            var parent1 = new NOctNode(2);

            var child1 = new NOctNode();
            parent1.SetChild(1, 1, 0, child1);
            Assert.AreEqual(parent1, child1.Parent);
            Assert.AreEqual(child1, parent1[1, 1, 0]);
            Assert.AreEqual(1, parent1.ChildCount);
            Assert.IsTrue(child1.IsLeaf);

            var child2 = new NOctNode();
            parent1.SetChild(1, 1, 0, child2);
            Assert.IsNull(child1.Parent);
            Assert.AreEqual(parent1, child2.Parent);
            Assert.AreEqual(child2, parent1[1, 1, 0]);
            Assert.AreEqual(1, parent1.ChildCount);
            Assert.IsTrue(child2.IsLeaf);

            parent1.SetChild(1, 1, 0, null);
            Assert.AreEqual(0, parent1.ChildCount);
            Assert.IsNull(child2.Parent);

            var parent2 = new NOctNode(1);

            parent2.SetChild(0, 0, 0, child1);
            Assert.AreEqual(parent2, child1.Parent);
            Assert.AreEqual(child1, parent2[0, 0, 0]);
            Assert.AreEqual(1, parent2.ChildCount);

            parent2.SetChild(0, 0, 1, child2);
            Assert.AreEqual(parent2, child2.Parent);
            Assert.AreEqual(child2, parent2[0, 0, 0]);
            Assert.AreEqual(2, parent2.ChildCount);

            parent2.SetChild(0, 1, 1, child2);
            Assert.AreEqual(parent2, child2.Parent);
            Assert.AreEqual(child2, parent2[0, 1, 1]);
            Assert.AreEqual(2, parent2.ChildCount);
        }

        [Test]
        public void NInParent()
        {
            // var root = new NOctNode(1);
            // var node = new NOctNode(2);
            // root.SetChild(NOctIndex.Zero, node);
            // var child = new NOctNode();
            // node.SetChild(NOctIndex.Zero, child);
            //
            // Assert.AreEqual(1, node.NInParent(root));
            // Assert.AreEqual(2, child.NInParent(node));
            // Assert.AreEqual(3, child.NInParent(root));
        }
    }
}
