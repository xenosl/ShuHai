using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Unity;
using ShuHai.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.Graphicode.Unity.UI
{
    using UObject = UnityEngine.Object;

    public class GraphPanel : Panel
    {
        public string Name
        {
            get => _graph != null ? _graph.Name : name;
            set
            {
                if (_graph != null)
                    _graph.Name = value;
                TitleText.text = value;
                name = value;
            }
        }

        protected GraphPanel() { _titleText = new Lazy<Text>(FindTitleText); }

        #region Graph Binding

        public Graph Graph
        {
            get => _graph;
            set
            {
                if (value == _graph)
                    return;

                if (_graph != null)
                    UnbindGraph();
                if (value != null)
                    BindGraph(value);
            }
        }

        private Graph _graph;

        private void BindGraph(Graph graph)
        {
            _graph = graph;
            _boundInstances.Add(graph, this);

            CreateElementEntities(_graph.Elements);
            _graph.ElementAdded += OnElementAdded;
            _graph.ElementRemoving += OnElementRemoving;
        }

        private void UnbindGraph()
        {
            _graph.ElementRemoving -= OnElementRemoving;
            _graph.ElementAdded -= OnElementAdded;
            DestroyElementEntities();

            _boundInstances.Remove(_graph, this);
            _graph = null;
        }

        private void OnElementAdded(Graph _, GraphElement element) { CreateElementEntity(element); }

        private void OnElementRemoving(Graph _, GraphElement element) { DestroyElementEntity(element); }

        private void EnsureGraphValid()
        {
            if (_graph == null)
                throw new InvalidOperationException("Failed to perform graph action since no graph is bound.");
        }

        #region Bound Instances

        public static IReadOnlyCollection<GraphPanel> GetBoundInstances(Graph graph)
        {
            return _boundInstances.GetValue(graph);
        }

        public static GraphPanel GetBoundInstance(Graph graph)
        {
            return _boundInstances.TryGetValue(graph, out var list) ? list[0] : null;
        }

        private static readonly Dictionary<Graph, List<GraphPanel>>
            _boundInstances = new Dictionary<Graph, List<GraphPanel>>();

        #endregion Bound Instances

        #endregion Graph Binding

        #region Elements

        public IReadOnlyDictionary<GraphElement, IGraphElementEntity> Elements => _elements;

        /// <summary>
        ///     Create an <see cref="Unit" /> of the specified type and add it to the bound <see cref="Graph" /> of current
        ///     <see cref="GraphPanel" />, then create a <see cref="UnitPanel" /> and bind the created <see cref="Unit" />
        ///     to it.
        /// </summary>
        /// <param name="unitType">Type of <see cref="Unit" /> to create.</param>
        /// <returns>The newly created <see cref="UnitPanel" />.</returns>
        public UnitPanel CreateUnitAndPanel(Type unitType)
        {
            Ensure.Argument.NotNull(unitType, nameof(unitType));
            Ensure.Argument.Is<Unit>(unitType, nameof(unitType));
            EnsureGraphValid();

            var unit = (Unit)Activator.CreateInstance(unitType);
            _graph.AddUnit(unit);

            return GetElementEntity<UnitPanel>(unit);
        }

        public bool DestroyUnitPanel(Unit unit)
        {
            Ensure.Argument.NotNull(unit, nameof(unit));
            EnsureGraphValid();

            return _graph.RemoveUnit(unit);
        }

        /// <summary>
        ///     Destroy all <see cref="IGraphElementEntity" />s in current <see cref="GraphPanel" /> and remove all
        ///     corresponding <see cref="GraphElement" />s from owner <see cref="Graph" />.
        /// </summary>
        public void DestroyElementEntities()
        {
            var elements = _elements.Keys.ToArray();
            foreach (var element in elements)
                DestroyElementEntity(element);
        }

        private readonly Dictionary<GraphElement, IGraphElementEntity>
            _elements = new Dictionary<GraphElement, IGraphElementEntity>();

        private IGraphElementEntity CreateElementEntity(GraphElement element)
        {
            var template = SelectElementEntityTemplate(element);
            var entity = (IGraphElementEntity)Instantiate(template, transform);
            entity.GraphElement = element;

            _elements.Add(element, entity);
            return entity;
        }

        private bool DestroyElementEntity(GraphElement element)
        {
            if (!_elements.TryGetValue(element, out var entity))
                return false;
            _elements.Remove(element);

            entity.GraphElement = null;

            switch (entity)
            {
                case Component component:
                    Destroy(component.gameObject);
                    break;
                case UObject uo:
                    Destroy(uo);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        private void CreateElementEntities(IEnumerable<GraphElement> elements)
        {
            foreach (var element in elements)
                CreateElementEntity(element);
        }

        private T GetElementEntity<T>(GraphElement element)
            where T : IGraphElementEntity
        {
            return (T)_elements.GetValue(element);
        }

        private bool TryGetElementEntity<T>(GraphElement element, out T entity)
            where T : IGraphElementEntity
        {
            entity = default;
            if (!_elements.TryGetValue(element, out var e))
                return false;

            if (!(e is T))
                return false;

            entity = (T)e;
            return true;
        }

        #region Entity Templates

        public UnitPanel UnitPanelTemplate;
        public ConnectionCurve ConnectionCurveTemplate;

        private UObject SelectElementEntityTemplate(GraphElement element)
        {
            switch (element)
            {
                case Unit _: return UnitPanelTemplate;
                case Connection _: return ConnectionCurveTemplate;
                case Comment _: throw new NotImplementedException();
                default:
                    throw new NotSupportedException($"Unsupported graph element '{element.GetType()}'.");
            }
        }

        #endregion Entity Templates

        #endregion Elements

        #region Child Components

        private Text TitleText => _titleText.Value;

        private readonly Lazy<Text> _titleText;

        private Text FindTitleText() { return gameObject.FindComponentInChild<Text>("Header/Title"); }

        #endregion Child Components

        #region Unity Events

        protected override void OnDestroy()
        {
            Graph = null;
            base.OnDestroy();
        }

        #endregion Unity Events
    }
}
