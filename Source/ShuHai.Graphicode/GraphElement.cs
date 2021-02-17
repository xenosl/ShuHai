using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public abstract class GraphElement
    {
        public string Name { get; set; }

        #region Constructors

        protected GraphElement() { Construct(GetType().Name); }
        protected GraphElement(string name) { Construct(name); }

        private void Construct(string name) { Name = name; }

        #endregion Constructors

        #region Owner Graph

        public Graph Owner { get; private set; }

        internal void _OnAddedToGraph(Graph graph)
        {
            Debug.Assert(Owner == null);
            Owner = graph;
            OnAddedToGraph();
        }

        protected virtual void OnAddedToGraph() { }

        internal void _OnRemovingFromGraph(Graph graph)
        {
            Debug.Assert(Owner == graph);
            OnRemovingFromGraph();
            Owner = null;
        }

        protected virtual void OnRemovingFromGraph() { }

        #endregion Owner Graph
    }
}