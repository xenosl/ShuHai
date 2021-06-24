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

        private static void NRankNode(int n, int expectedChildCapacity, int expectedDimensionalChildCapacity)
        {
            var node = new NOctNode(n);
            Assert.AreEqual(n, node.N);
            Assert.AreEqual(0, node.ChildCount);
            Assert.AreEqual(expectedChildCapacity, node.ChildCapacity);
            Assert.AreEqual(expectedDimensionalChildCapacity, node.DimensionalChildCapacity);

            Assert.AreEqual(NOctIndex.Invalid, node.Index);

            Assert.IsNull(node.Parent);
            Assert.IsTrue(node.IsRoot);
            Assert.AreEqual(node, node.FindRoot());

            Assert.IsTrue(node.IsLeaf);
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
        public void DepthOfChild()
        {
            var root = new NOctNode(1);
            var child1 = new NOctNode(2);
            root.SetChild(NOctIndex.Zero, child1);
            Assert.AreEqual(1, root.DepthOf(child1));

            var child2 = new NOctNode(3);
            child1.SetChild(NOctIndex.Zero, child2);
            Assert.AreEqual(1, child1.DepthOf(child2));
            Assert.AreEqual(2, root.DepthOf(child2));

            var child3 = new NOctNode(1);
            child2.SetChild(NOctIndex.Zero, child3);
            Assert.AreEqual(1, child2.DepthOf(child3));
            Assert.AreEqual(2, child1.DepthOf(child3));
            Assert.AreEqual(3, root.DepthOf(child3));
        }

        [Test]
        public void RankOfChild()
        {
            var root = new NOctNode(1);
            var child1 = new NOctNode(2);
            root.SetChild(NOctIndex.Zero, child1);
            Assert.AreEqual(1, root.RankOf(child1));

            var child2 = new NOctNode(3);
            child1.SetChild(NOctIndex.Zero, child2);
            Assert.AreEqual(2, child1.RankOf(child2));
            Assert.AreEqual(3, root.RankOf(child2));

            var child3 = new NOctNode(1);
            child2.SetChild(NOctIndex.Zero, child3);
            Assert.AreEqual(3, child2.RankOf(child3));
            Assert.AreEqual(5, child1.RankOf(child3));
            Assert.AreEqual(6, root.RankOf(child3));
        }

        [Test]
        public void IndexOfChild()
        {
            var root = new NOctNode(1);
            var child1 = new NOctNode(2);
            root.SetChild(new NOctIndex(1, 0, 1), child1);
            Assert.AreEqual(child1.Index, root.IndexOf(child1));
            
            // var child2 = new NOctNode(3);
            // child1.SetChild(new NOctIndex(2, 1, 3), child2);
            // Assert.AreEqual(new NOctIndex(2, 3, 5), child1.IndexOf(child2));
            // Assert.AreEqual(new NOctIndex(4 * 1 + 2, 4 * 0 + 1, 4 * 1 + 3), root.IndexOf(child2));
            //
            // var child3 = new NOctNode(1);
            // child2.SetChild(new NOctIndex(0, 1, 1), child3);
            // Assert.AreEqual(new NOctIndex(0, 1, 1), child2.IndexOf(child3));
            // Assert.AreEqual(new NOctIndex(), child1.IndexOf(child3));
            // Assert.AreEqual(new NOctIndex(), root.IndexOf(child3));
        }
    }
}
