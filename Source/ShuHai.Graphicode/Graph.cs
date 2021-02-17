using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public class Graph
    {
        public string Name;

        public Graph() : this(typeof(Graph).Name) { }
        public Graph(string name) { Name = name; }

        #region Elements

        /// <summary>
        ///     Occurs after a <see cref="GraphElement" /> instance is added to current <see cref="Graph" />.
        /// </summary>
        public event Action<Graph, GraphElement> ElementAdded;

        /// <summary>
        ///     Occurs before a <see cref="GraphElement" /> instance remove from current <see cref="Graph" />.
        /// </summary>
        public event Action<Graph, GraphElement> ElementRemoving;

        /// <summary>
        ///     A collection that contains all <see cref="GraphElement" /> instances which belongs to current
        ///     <see cref="Graph" /> instance.
        /// </summary>
        public IReadOnlyCollection<GraphElement> Elements => _elements;

        /// <summary>
        ///     Get a value indicates whether the specified <see cref="GraphElement" /> is part of current graph.
        /// </summary>
        /// <param name="element">The element instance to test.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified <see cref="GraphElement" /> is part of current graph; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        public bool Contains(GraphElement element) { return _elements.Contains(element); }

        /// <summary>
        ///     Add the specified unit to current graph.
        /// </summary>
        /// <param name="unit">The <see cref="Unit" /> instance to add.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified unit is added to current graph; otherwise, <see langword="false" />
        ///     if the specified unit is already belong to current graph.
        /// </returns>
        public bool AddUnit(Unit unit) { return AddElement(unit); }

        /// <summary>
        ///     Remove the specified unit from current graph.
        /// </summary>
        /// <param name="unit">The <see cref="Unit" /> instance to remove.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified unit is removed from current graph; otherwise, <see langword="false" />
        ///     if the specified unit does not belong to current graph.
        /// </returns>
        public bool RemoveUnit(Unit unit) { return RemoveElement(unit); }

        /// <summary>
        ///     Event method for derived classes automatically invoked after any element is added to current graph. Note
        ///     that the method is invoked before the public event <see cref="ElementAdded" />.
        /// </summary>
        /// <param name="element">The newly added element.</param>
        protected virtual void OnElementAdded(GraphElement element) { }

        /// <summary>
        ///     Event method for derived classes automatically invoked before any element is removing from current graph.
        ///     Note that the method is invoked after the public event <see cref="ElementRemoving" />.
        /// </summary>
        /// <param name="element">The element about to remove.</param>
        protected virtual void OnElementRemoving(GraphElement element) { }

        internal bool AddElement(GraphElement element)
        {
            Ensure.Argument.NotNull(element, nameof(element));

            var owner = element.Owner;
            if (owner != null)
            {
                if (owner == this)
                    return false;

                throw new InvalidOperationException(
                    "Attempt to add element which belongs to another graph to current graph.");
            }

            bool added = _elements.Add(element);
            Debug.Assert(added);
            element._OnAddedToGraph(this);

            OnElementAdded(element);
            ElementAdded?.Invoke(this, element);

            return true;
        }

        internal bool RemoveElement(GraphElement element)
        {
            Ensure.Argument.NotNull(element, nameof(element));

            if (element.Owner != this)
                return false;

            ElementRemoving?.Invoke(this, element);
            OnElementRemoving(element);

            element._OnRemovingFromGraph(this);
            bool removed = _elements.Remove(element);
            Debug.Assert(removed);

            return true;
        }

        private readonly HashSet<GraphElement> _elements = new HashSet<GraphElement>();

        #endregion Elements
    }
}
