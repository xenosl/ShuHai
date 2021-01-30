using System;
using NUnit.Framework;

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
            //ConvertTest(graph);
        }
    }
}