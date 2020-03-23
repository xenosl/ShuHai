using NUnit.Framework;
using ShuHai.XConverts;

namespace ShuHai.Graphicode.Serialization.Tests
{
    public class GraphTests
    {
        [Test]
        public void Serialization()
        {
            var graph = new Graph();
            graph.AddUnit(new BranchUnit());
            graph.AddUnit(new ForLoopUnit());
            ConvertTest(graph);
        }

        private static void ConvertTest(Graph graph, XConvertSettings settings = null)
        {
            var elem = XConvert.ToXElement(graph, "ConvertTest", settings);
            var obj = (Graph)XConvert.ToObject(elem, settings);
            Assert.IsTrue(obj.Name == graph.Name);
            //CollectionAssert.AreEquivalent(obj.Elements, graph.Elements);
            Assert.AreEqual(obj.Elements.Count, graph.Elements.Count);
        }
    }
}