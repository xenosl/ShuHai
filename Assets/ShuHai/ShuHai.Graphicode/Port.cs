using System;
using System.Collections.Generic;
using System.Text;

namespace ShuHai.Graphicode
{
    internal abstract class Port : IPort
    {
        public Unit Owner { get; }

        public string Name { get; set; }

        protected Port(Unit owner)
        {
            Ensure.Argument.NotNull(owner, nameof(owner));
            Owner = owner;
        }

        #region Connections

        public abstract int ConnectedPortCount { get; }
        public IEnumerable<Connection> Connections => EnumerateConnections();

        protected abstract IEnumerable<Connection> EnumerateConnections();

        #region Connect

        public abstract bool CanConnect(IPort other);
        public abstract Connection Connect(IPort other);

        protected static bool CanConnect<TOther, TDesired>(TOther other, Func<TDesired, bool> func)
            where TOther : IPort
            where TDesired : IPort
        {
            return other is TDesired desiredOther && func(desiredOther);
        }

        protected static TConnection Connect<TConnection, TOther, TDesired>(
            TOther other, Func<TDesired, TConnection> func)
            where TConnection : Connection
            where TOther : IPort
            where TDesired : IPort
        {
            if (other is TDesired desiredOther)
                return func(desiredOther);
            return null;
        }

        #endregion Connect

        public abstract bool Disconnect(IPort other);

        public static bool Disconnect<TBase, TActual>(TBase other, Func<TActual, bool> func)
            where TBase : IPort
            where TActual : IPort
        {
            if (other is TActual p)
                return func(p);
            return false;
        }

        #endregion Connections

        public override string ToString()
        {
            var b = new StringBuilder();

            if (Owner != null)
                b.Append(Owner).Append('.');

            b.Append(GetType().Name);
            if (!string.IsNullOrEmpty(Name))
                b.Append('(').Append(Name).Append(')');

            return b.ToString();
        }
    }

    #region Value Ports

    internal abstract class ValuePort<T> : Port, IValuePort<T>
    {
        #region Value

        public T Value { get; set; }

        protected internal ValuePort(Unit owner) : base(owner) { }

        Type IValuePort.ValueType => typeof(T);

        object IValuePort.Value
        {
            get => Value;
            set => Value = (T)value;
        }

        #endregion Value

        #region Connections

        public sealed override int ConnectedPortCount => ConnectionDict.Count;
        public new IEnumerable<ValueConnection> Connections => ValueConnections;

        protected abstract IEnumerable<ValueConnection> ValueConnections { get; }

        protected sealed override IEnumerable<Connection> EnumerateConnections() { return ValueConnections; }

        protected readonly Dictionary<ConnectionKey, ValueConnection>
            ConnectionDict = new Dictionary<ConnectionKey, ValueConnection>();

        #region Connect

        public sealed override bool CanConnect(IPort other) => CanConnect<IPort, IValuePort>(other, CanConnect);

        public sealed override Connection Connect(IPort other) =>
            Connect<ValueConnection, IPort, IValuePort>(other, Connect);

        public abstract bool CanConnect(IValuePort other);
        public abstract ValueConnection Connect(IValuePort other);

        protected virtual void PostConnect(ValueConnection connection)
        {
            ConnectionDict.Add(connection.Key, connection);
        }

        #endregion Connect

        #region Disconnect

        public abstract bool Disconnect(IValuePort other);

        public sealed override bool Disconnect(IPort other) { return Disconnect<IPort, IValuePort>(other, Disconnect); }

        protected virtual void PreDisconnect(ValueConnection connection) { ConnectionDict.Remove(connection.Key); }

        #endregion Disconnect

        #endregion Connections
    }

    internal sealed class InputValuePort<T> : ValuePort<T>, IInputValuePort<T>
    {
        public int InputIndex { get; }

        public int InputValueIndex { get; }

        internal InputValuePort(Unit owner, int inputIndex, int valueIndex, T value = default)
            : base(owner)
        {
            InputIndex = inputIndex;
            InputValueIndex = valueIndex;
            Value = value;
        }

        #region Connections

        public new IEnumerable<ValueConnection> Connections => ConnectionDict.Values;

        protected sealed override IEnumerable<ValueConnection> ValueConnections => Connections;

        IEnumerable<Connection> IInputPort.Connections => Connections;

        #region Connect

        public bool CanConnect(IOutputPort other) => CanConnect<IOutputPort, IOutputValuePort>(other, CanConnect);

        public Connection Connect(IOutputPort other) =>
            Connect<ValueConnection, IOutputPort, IOutputValuePort>(other, Connect);

        public sealed override bool CanConnect(IValuePort other) =>
            CanConnect<IValuePort, IOutputValuePort>(other, CanConnect);

        public sealed override ValueConnection Connect(IValuePort other) =>
            Connect<ValueConnection, IValuePort, IOutputValuePort>(other, Connect);

        public bool CanConnect(IOutputValuePort other) { return ValueConnection.CanConnect(other, this); }

        public ValueConnection Connect(IOutputValuePort other)
        {
            return ValueConnection.ConnectImpl(other, this, PostConnect);
        }

        #endregion Connect

        #region Dicconnect

        public bool Disconnect(IOutputValuePort other)
        {
            return ValueConnection.DisconnectImpl(other, this, PreDisconnect);
        }

        public bool Disconnect(IOutputPort other)
        {
            return Disconnect<IOutputPort, IOutputValuePort>(other, Disconnect);
        }

        public sealed override bool Disconnect(IValuePort other)
        {
            return Disconnect<IValuePort, IOutputValuePort>(other, Disconnect);
        }

        #endregion Dicconnect

        #endregion Connections
    }

    internal sealed class OutputValuePort<T> : ValuePort<T>, IOutputValuePort<T>
    {
        public int OutputIndex { get; }

        public int OutputValueIndex { get; }

        internal OutputValuePort(Unit owner, int outputIndex, int valueIndex)
            : base(owner)
        {
            OutputIndex = outputIndex;
            OutputValueIndex = valueIndex;
        }

        #region Connections

        public new IEnumerable<ValueConnection> Connections => ConnectionDict.Values;

        protected sealed override IEnumerable<ValueConnection> ValueConnections => Connections;

        IEnumerable<Connection> IOutputPort.Connections => Connections;

        #region Connect

        public bool CanConnect(IInputPort other) => CanConnect<IInputPort, IInputValuePort>(other, CanConnect);

        public Connection Connect(IInputPort other) =>
            Connect<ValueConnection, IInputPort, IInputValuePort>(other, Connect);

        public sealed override bool CanConnect(IValuePort other) =>
            CanConnect<IValuePort, IInputValuePort>(other, CanConnect);

        public sealed override ValueConnection Connect(IValuePort other) =>
            Connect<ValueConnection, IValuePort, IInputValuePort>(other, Connect);

        public bool CanConnect(IInputValuePort other) { return ValueConnection.CanConnect(this, other); }

        public ValueConnection Connect(IInputValuePort other)
        {
            return ValueConnection.ConnectImpl(this, other, PostConnect);
        }

        #endregion Connect

        #region Disconnect

        public bool Disconnect(IInputValuePort other)
        {
            return ValueConnection.DisconnectImpl(this, other, PreDisconnect);
        }

        public bool Disconnect(IInputPort other) { return Disconnect<IInputPort, IInputValuePort>(other, Disconnect); }

        public sealed override bool Disconnect(IValuePort other)
        {
            return Disconnect<IValuePort, IInputValuePort>(other, Disconnect);
        }

        #endregion Disconnect

        #endregion Connections
    }

    #endregion Value Ports

    #region Flow Ports

    internal abstract class FlowPort : Port, IFlowPort
    {
        public new FlowUnit Owner => (FlowUnit)base.Owner;

        public abstract void Flow();

        protected FlowPort(Unit owner) : base(owner) { }

        #region Connections

        public sealed override int ConnectedPortCount => ConnectionDict.Count;
        public new IEnumerable<FlowConnection> Connections => FlowConnections;

        protected sealed override IEnumerable<Connection> EnumerateConnections() { return FlowConnections; }

        protected abstract IEnumerable<FlowConnection> FlowConnections { get; }

        protected readonly Dictionary<ConnectionKey, FlowConnection>
            ConnectionDict = new Dictionary<ConnectionKey, FlowConnection>();

        #region Connect

        public sealed override bool CanConnect(IPort other) => CanConnect<IPort, IFlowPort>(other, CanConnect);

        public sealed override Connection Connect(IPort other) =>
            Connect<FlowConnection, IPort, IFlowPort>(other, Connect);

        public abstract bool CanConnect(IFlowPort other);

        public abstract FlowConnection Connect(IFlowPort other);

        protected virtual void PostConnect(FlowConnection connection)
        {
            ConnectionDict.Add(connection.Key, connection);
        }

        #endregion Connect

        #region Disconnect

        public abstract bool Disconnect(IFlowPort other);

        public sealed override bool Disconnect(IPort other) { return Disconnect<IPort, IFlowPort>(other, Disconnect); }

        protected virtual void PreDisconnect(FlowConnection connection) { ConnectionDict.Remove(connection.Key); }

        #endregion Disconnect

        #endregion Connections
    }

    internal sealed class InputFlowPort : FlowPort, IInputFlowPort
    {
        public int InputIndex { get; }

        public int InputFlowIndex { get; }

        public override void Flow() { Owner.Execute(InputFlowIndex); }

        internal InputFlowPort(Unit owner, int inputIndex, int flowIndex)
            : base(owner)
        {
            InputIndex = inputIndex;
            InputFlowIndex = flowIndex;
        }

        #region Connections

        public new IEnumerable<FlowConnection> Connections => ConnectionDict.Values;

        protected sealed override IEnumerable<FlowConnection> FlowConnections => Connections;

        IEnumerable<Connection> IInputPort.Connections => Connections;

        #region Connect

        public override bool CanConnect(IFlowPort other) => CanConnect<IFlowPort, IOutputFlowPort>(other, CanConnect);

        public override FlowConnection Connect(IFlowPort other) =>
            Connect<FlowConnection, IFlowPort, IOutputFlowPort>(other, Connect);

        public bool CanConnect(IOutputPort other) => CanConnect<IOutputPort, IOutputFlowPort>(other, CanConnect);

        public Connection Connect(IOutputPort other) =>
            Connect<FlowConnection, IOutputPort, IOutputFlowPort>(other, Connect);

        public bool CanConnect(IOutputFlowPort other) { return FlowConnection.CanConnect(other, this); }

        public FlowConnection Connect(IOutputFlowPort other)
        {
            return FlowConnection.ConnectImpl(other, this, PostConnect);
        }

        #endregion Connect

        #region Disconnect

        public bool Disconnect(IOutputFlowPort other)
        {
            return FlowConnection.DisconnectImpl(other, this, PreDisconnect);
        }

        public bool Disconnect(IOutputPort other)
        {
            return Disconnect<IOutputPort, IOutputFlowPort>(other, Disconnect);
        }

        public override bool Disconnect(IFlowPort other)
        {
            return Disconnect<IFlowPort, IOutputFlowPort>(other, Disconnect);
        }

        #endregion Disconnect

        #endregion Connections
    }

    internal sealed class OutputFlowPort : FlowPort, IOutputFlowPort
    {
        public int OutputIndex { get; }

        public int OutputFlowIndex { get; }

        public override void Flow()
        {
            foreach (var port in this.GetConnectedPorts())
                port.Flow();
        }

        internal OutputFlowPort(Unit owner, int outputIndex, int flowIndex)
            : base(owner)
        {
            OutputIndex = outputIndex;
            OutputFlowIndex = flowIndex;
        }

        #region Connections

        public new IEnumerable<FlowConnection> Connections => ConnectionDict.Values;

        protected sealed override IEnumerable<FlowConnection> FlowConnections => Connections;

        IEnumerable<Connection> IOutputPort.Connections => Connections;

        #region Connect

        public override bool CanConnect(IFlowPort other) => CanConnect<IFlowPort, IInputFlowPort>(other, CanConnect);

        public override FlowConnection Connect(IFlowPort other) =>
            Connect<FlowConnection, IFlowPort, IInputFlowPort>(other, Connect);

        public bool CanConnect(IInputPort other) => CanConnect<IInputPort, IInputFlowPort>(other, CanConnect);

        public Connection Connect(IInputPort other) =>
            Connect<FlowConnection, IInputPort, IInputFlowPort>(other, Connect);

        public bool CanConnect(IInputFlowPort other) { return FlowConnection.CanConnect(this, other); }

        public FlowConnection Connect(IInputFlowPort other)
        {
            return FlowConnection.ConnectImpl(this, other, PostConnect);
        }

        #endregion Connect

        #region Disconnect

        public bool Disconnect(IInputFlowPort other)
        {
            return FlowConnection.DisconnectImpl(this, other, PreDisconnect);
        }

        public bool Disconnect(IInputPort other) { return Disconnect<IInputPort, IInputFlowPort>(other, Disconnect); }

        public sealed override bool Disconnect(IFlowPort other)
        {
            return Disconnect<IFlowPort, IInputFlowPort>(other, Disconnect);
        }

        #endregion Disconnect

        #endregion Connections
    }

    #endregion Flow Ports
}
