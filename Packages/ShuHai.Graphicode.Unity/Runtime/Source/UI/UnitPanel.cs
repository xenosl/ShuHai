using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Unity;
using ShuHai.Unity.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace ShuHai.Graphicode.Unity.UI
{
    public class UnitPanel : Panel, IGraphElementEntity
    {
        public string Name
        {
            get => Unit != null ? Unit.Name : name;
            set
            {
                if (Unit != null)
                    Unit.Name = value;
                TitleText.text = value;
                name = value;
            }
        }

        public override string ToString()
        {
            var body = Unit != null ? Unit.ToString() : GetType().Name;
            return string.IsNullOrEmpty(Name) ? body : $"{body}({Name})";
        }

        protected UnitPanel()
        {
            _titleText = new Lazy<Text>(FindTitleText);
            _inputPortsRect = new Lazy<RectTransform>(FindInputPortsRect);
            _outputPortsRect = new Lazy<RectTransform>(FindOutputPortsRect);
        }

        #region Unit Binding

        public Unit Unit
        {
            get => _unit;
            set
            {
                if (value == _unit)
                    return;

                if (_unit != null)
                    UnbindUnit();
                if (value != null)
                    BindUnit(value);
            }
        }

        private Unit _unit;

        private void BindUnit(Unit unit)
        {
            _unit = unit;
            _boundInstances.Add(_unit, this);

            Name = _unit.Name;

            CreatePortWidgets(_unit.Ports);
            _unit.PortAdded += OnPortAdded;
            _unit.PortRemoving += OnPortRemoving;
        }

        private void UnbindUnit()
        {
            _unit.PortRemoving -= OnPortRemoving;
            _unit.PortAdded -= OnPortAdded;
            DestroyPortWidgets();

            Name = GetType().Name;

            _boundInstances.Remove(_unit);
            _unit = null;
        }

        private void OnPortAdded(Unit unit, IPort port) { CreatePortWidget(port); }

        private void OnPortRemoving(Unit unit, IPort port) { DestroyPortWidget(port); }

        #region Bound Instances

        /// <summary>
        ///     All <see cref="UnitPanel" /> instances already bound to an <see cref="Unit" />.
        /// </summary>
        public static IReadOnlyDictionary<Unit, UnitPanel> BoundInstances => _boundInstances;

        private static readonly Dictionary<Unit, UnitPanel> _boundInstances = new Dictionary<Unit, UnitPanel>();

        #endregion Bound Instances

        #endregion Unit Binding

        #region Port Widgets

        public PortWidget InputPortWidgetTemplate;

        public PortWidget OutputPortWidgetTemplate;

        public IReadOnlyDictionary<IPort, PortWidget> PortWidgets => _portWidgets;

        private readonly Dictionary<IPort, PortWidget> _portWidgets = new Dictionary<IPort, PortWidget>();

        private void CreatePortWidgets(IEnumerable<IPort> ports)
        {
            foreach (var port in ports)
                CreatePortWidget(port);
        }

        private void DestroyPortWidgets()
        {
            var ports = _portWidgets.Keys.ToArray();
            foreach (var port in ports)
                DestroyPortWidget(port);
        }

        private void CreatePortWidget(IPort port)
        {
            var prefab = SelectPortWidgetPrefab(port);
            if (!prefab)
                throw new UnassignedReferenceException("PortWidget prefab is required to create a PortWidget.");

            var widget = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            widget.Port = port;
            _portWidgets.Add(port, widget);

            Layouter.MarkDirty();
        }

        private void DestroyPortWidget(IPort port)
        {
            var widget = _portWidgets[port];

            _portWidgets.Remove(port);
            widget.Port = null;
            Destroy(widget.gameObject);

            Layouter.MarkDirty();
        }

        private PortWidget SelectPortWidgetPrefab(IPort port)
        {
            switch (port)
            {
                case IInputPort _:
                    return InputPortWidgetTemplate;
                case IOutputPort _:
                    return OutputPortWidgetTemplate;
                default:
                    throw new InvalidProgramException();
            }
        }

        internal RectTransform SelectPortRect(IPort port)
        {
            switch (port)
            {
                case IInputPort _:
                    return InputPortsRect;
                case IOutputPort _:
                    return OutputPortsRect;
                default:
                    throw new InvalidProgramException();
            }
        }

        #endregion Port Widgets

        #region Child Components

        private Text TitleText => _titleText.Value;

        private RectTransform InputPortsRect => _inputPortsRect.Value;
        private RectTransform OutputPortsRect => _outputPortsRect.Value;

        private readonly Lazy<Text> _titleText;

        private readonly Lazy<RectTransform> _inputPortsRect;
        private readonly Lazy<RectTransform> _outputPortsRect;

        private Text FindTitleText() { return gameObject.FindComponentInChild<Text>("Header/Title"); }

        private RectTransform FindInputPortsRect()
        {
            return gameObject.FindComponentInChild<RectTransform>("Ports/Inputs");
        }

        private RectTransform FindOutputPortsRect()
        {
            return gameObject.FindComponentInChild<RectTransform>("Ports/Outputs");
        }

        #endregion Child Components

        protected override void OnDestroy()
        {
            Unit = null;
            base.OnDestroy();
        }

        GraphElement IGraphElementEntity.GraphElement
        {
            get => Unit;
            set => Unit = (Unit)value;
        }
    }
}