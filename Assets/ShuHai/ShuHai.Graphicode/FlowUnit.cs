using System;
using System.Collections.Generic;

namespace ShuHai.Graphicode
{
    public abstract class FlowUnit : Unit
    {
        protected FlowUnit() : this(typeof(FlowUnit).Name) { }
        protected FlowUnit(string name) : base(name) { }

        #region Execution

        /// <summary>
        ///     Execute the defined logic of current instance based on specified flow port and values of input ports, then
        ///     set output values to output ports.
        /// </summary>
        public void Execute(int inputFlowIndex)
        {
            try
            {
                if (InputFlowPorts.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Attempt to execute flow unit without any input flow port defined");
                }
                ExecuteImpl(InputFlowPorts[inputFlowIndex]);
            }
            catch (Exception e)
            {
                OnExecuteException(e);
            }
        }

        protected abstract void ExecuteImpl(IInputFlowPort byPort);

        /// <summary>
        ///     Shortcut for <see cref="Execute(int)" /> with parameter set to 0.
        /// </summary>
        public sealed override void Execute() { Execute(0); }

        protected sealed override void ExecuteImpl() { ExecuteImpl(InputFlowPorts[0]); }

        #endregion Execution

        #region Ports

        public IReadOnlyList<IInputFlowPort> InputFlowPorts => _inputFlowPorts;
        public IReadOnlyList<IOutputFlowPort> OutputFlowPorts => _outputFlowPorts;

        public IInputFlowPort AddInputFlowPort(string name = "")
        {
            return AddPort(new InputFlowPort(this, InputPorts.Count, InputFlowPorts.Count) { Name = name });
        }

        public IOutputFlowPort AddOutputFlowPort(string name = "")
        {
            return AddPort(new OutputFlowPort(this, OutputPorts.Count, OutputFlowPorts.Count) { Name = name });
        }

        protected override void AddPortImpl(IPort port)
        {
            base.AddPortImpl(port);

            switch (port)
            {
                case IInputFlowPort inputFlowPort:
                    _inputFlowPorts.Add(inputFlowPort);
                    break;
                case IOutputFlowPort outputFlowPort:
                    _outputFlowPorts.Add(outputFlowPort);
                    break;
            }
        }

        protected override void RemovePortImpl(IPort port)
        {
            switch (port)
            {
                case IInputFlowPort inputFlowPort:
                    _inputFlowPorts.Remove(inputFlowPort);
                    break;
                case IOutputFlowPort outputFlowPort:
                    _outputFlowPorts.Remove(outputFlowPort);
                    break;
            }

            base.RemovePortImpl(port);
        }

        private readonly List<IInputFlowPort> _inputFlowPorts = new List<IInputFlowPort>();
        private readonly List<IOutputFlowPort> _outputFlowPorts = new List<IOutputFlowPort>();

        #endregion Ports
    }
}