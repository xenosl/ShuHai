using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShuHai.Graphicode
{
    /// <summary>
    ///     Represents an execution unit actually done the work.
    /// </summary>
    public abstract class Unit : GraphElement
    {
        protected Unit() : this(nameof(Unit)) { }
        protected Unit(string name) : base(name) { }

        public override string ToString()
        {
            var body = GetType().Name;
            return string.IsNullOrEmpty(Name) ? body : $"{body}({Name})";
        }

        #region Execution

        /// <summary>
        ///     Occurs when any exception thrown while executing.
        /// </summary>
        public event Action<Unit, Exception> ExceptionOnExecute;

        /// <summary>
        ///     Execute the defined logic of current unit instance based on values of input ports, then set output values
        ///     to output ports.
        /// </summary>
        public virtual void Execute()
        {
            try
            {
                ExecuteImpl();
            }
            catch (Exception e)
            {
                OnExecuteException(e);
            }
        }

        protected abstract void ExecuteImpl();

        protected virtual void OnExecuteException(Exception e) { ExceptionOnExecute?.Invoke(this, e); }

        #endregion Execution

        #region Ports

        /// <summary>
        ///     Occurs after amy port is added to current unit.
        /// </summary>
        public event Action<Unit, IPort> PortAdded;

        /// <summary>
        ///     Occurs when any port is about to removing from current unit.
        /// </summary>
        public event Action<Unit, IPort> PortRemoving;

        /// <summary>
        ///     All ports defined in current unit.
        /// </summary>
        public IReadOnlyCollection<IPort> Ports => _ports;

        public IReadOnlyList<IInputPort> InputPorts => _inputPorts;
        public IReadOnlyList<IOutputPort> OutputPorts => _outputPorts;

        public IReadOnlyList<IInputValuePort> InputValuePorts => _inputValuePorts;
        public IReadOnlyList<IOutputValuePort> OutputValuePorts => _outputValuePorts;

        /// <summary>
        ///     Create an input port for value read/write and add it to current unit.
        /// </summary>
        /// <param name="name">The name of the port to create.</param>
        /// <typeparam name="T">Type of value managed by the port.</typeparam>
        /// <returns>The newly created port instance.</returns>
        public IInputValuePort<T> AddInputValuePort<T>(string name = "")
        {
            return AddPort(new InputValuePort<T>(this, InputPorts.Count, InputValuePorts.Count) { Name = name });
        }

        /// <summary>
        ///     Create an output port for value read/write and add it to current unit.
        /// </summary>
        /// <param name="name">The name of the port to create.</param>
        /// <typeparam name="T">Type of value managed by the port.</typeparam>
        /// <returns>The newly created port instance.</returns>
        public IOutputValuePort<T> AddOutputValuePort<T>(string name = "")
        {
            return AddPort(new OutputValuePort<T>(this, OutputPorts.Count, OutputValuePorts.Count) { Name = name });
        }

        protected T AddPort<T>(T port)
            where T : IPort
        {
            AddPortImpl(port);
            PortAdded?.Invoke(this, port);
            return port;
        }

        public bool RemovePort(IPort port)
        {
            if (_ports.Contains(port))
                return false;
            PortRemoving?.Invoke(this, port);
            RemovePortImpl(port);
            return true;
        }

        /// <summary>
        ///     Actually add the specified port to corresponding collection of current unit.
        /// </summary>
        /// <param name="port">The port to add.</param>
        protected virtual void AddPortImpl(IPort port)
        {
            bool added = _ports.Add(port);
            Debug.Assert(added);

            switch (port)
            {
                case IInputPort inputPort:
                    _inputPorts.Add(inputPort);
                    if (inputPort is IInputValuePort inputValuePort)
                        _inputValuePorts.Add(inputValuePort);
                    break;
                case IOutputPort outputPort:
                    _outputPorts.Add(outputPort);
                    if (outputPort is IOutputValuePort outputValuePort)
                        _outputValuePorts.Add(outputValuePort);
                    break;
            }
        }

        /// <summary>
        ///     Actually remove the specified port from corresponding collection of current unit.
        /// </summary>
        /// <param name="port">The port to remove.</param>
        protected virtual void RemovePortImpl(IPort port)
        {
            switch (port)
            {
                case IInputPort inputPort:
                    if (inputPort is IInputValuePort inputValuePort)
                        _inputValuePorts.Remove(inputValuePort);
                    _inputPorts.Remove(inputPort);
                    break;
                case IOutputPort outputPort:
                    if (outputPort is IOutputValuePort outputValuePort)
                        _outputValuePorts.Remove(outputValuePort);
                    _outputPorts.Remove(outputPort);
                    break;
            }

            bool removed = _ports.Remove(port);
            Debug.Assert(removed);
        }

        private readonly HashSet<IPort> _ports = new HashSet<IPort>();

        private readonly List<IInputPort> _inputPorts = new List<IInputPort>();
        private readonly List<IOutputPort> _outputPorts = new List<IOutputPort>();

        private readonly List<IInputValuePort> _inputValuePorts = new List<IInputValuePort>();
        private readonly List<IOutputValuePort> _outputValuePorts = new List<IOutputValuePort>();

        #endregion Ports

        #region Owner Graph

        protected override void OnRemovingFromGraph()
        {
            DisconnectAllPorts();
            base.OnRemovingFromGraph();
        }

        private void DisconnectAllPorts()
        {
            var connections = _ports.SelectMany(p => p.Connections).ToArray();
            foreach (var conn in connections)
                conn.Disconnect();
        }

        #endregion Owner Graph
    }
}
