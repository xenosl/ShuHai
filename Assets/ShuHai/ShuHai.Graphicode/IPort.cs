using System;
using System.Collections.Generic;

namespace ShuHai.Graphicode
{
    /// <summary>
    ///     Represents a port owned by a <see cref="Unit" />.
    /// </summary>
    public interface IPort
    {
        /// <summary>
        ///     The owner <see cref="Unit" /> of current instance.
        /// </summary>
        Unit Owner { get; }

        /// <summary>
        ///     Name of current instance.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Number of ports connected to current instance.
        /// </summary>
        int ConnectedPortCount { get; }

        /// <summary>
        ///     An enumerable object that enumerates all connections connecting current port with another.
        /// </summary>
        IEnumerable<Connection> Connections { get; }

        /// <summary>
        ///     Get a value indicates whether the current port is able to connect to the specified port.
        /// </summary>
        /// <param name="other">The port to test.</param>
        /// <returns>
        ///     <see langword="true" /> if the current port is able to connect to the specified port; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        bool CanConnect(IPort other);

        /// <summary>
        ///     Connect current port to the specified port.
        /// </summary>
        /// <param name="other">The port to connect.</param>
        /// <returns>
        ///     A <see cref="Connection" /> object represents the connection between current port and the specified port,
        ///     if succeed; otherwise, <see langword="null" /> if the current port is unable to connect to the specified
        ///     port.
        /// </returns>
        Connection Connect(IPort other);

        /// <summary>
        ///     Disconnect the connection between current port and the specified port.
        /// </summary>
        /// <param name="other">The port current port connect with.</param>
        /// <returns>
        ///     <see langword="true" /> if the connection connects current port and the specified port is successfully
        ///     disconnected;
        ///     otherwise, <see langword="false" /> if the connection does not exist.
        /// </returns>
        bool Disconnect(IPort other);
    }

    public interface IInputPort : IPort
    {
        /// <summary>
        ///     Index of current port in <see cref="Unit.InputPorts " />.
        /// </summary>
        int InputIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<Connection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IOutputPort other);

        /// <inheritdoc cref="IPort.Connect" />
        Connection Connect(IOutputPort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IOutputPort other);
    }

    public interface IOutputPort : IPort
    {
        /// <summary>
        ///     Index of current port in <see cref="Unit.OutputPorts " />.
        /// </summary>
        int OutputIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<Connection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IInputPort other);

        /// <inheritdoc cref="IPort.CanConnect" />
        Connection Connect(IInputPort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IInputPort other);
    }

    #region Value Ports

    public interface IValuePort : IPort
    {
        object Value { get; set; }

        /// <summary>
        ///     Type of value the current port is supported.
        /// </summary>
        Type ValueType { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<ValueConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IValuePort other);

        /// <inheritdoc cref="IPort.CanConnect" />
        ValueConnection Connect(IValuePort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IValuePort other);
    }

    public interface IInputValuePort : IValuePort, IInputPort
    {
        /// <summary>
        ///     Index of current port in <see cref="Unit.InputValuePorts " />.
        /// </summary>
        int InputValueIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<ValueConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IOutputValuePort other);

        /// <inheritdoc cref="IPort.Connect" />
        ValueConnection Connect(IOutputValuePort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IOutputValuePort other);
    }

    public interface IOutputValuePort : IValuePort, IOutputPort
    {
        /// <summary>
        ///     Index of current port in <see cref="Unit.OutputValuePorts " />.
        /// </summary>
        int OutputValueIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<ValueConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IInputValuePort other);

        /// <inheritdoc cref="IPort.Connect" />
        ValueConnection Connect(IInputValuePort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IInputValuePort other);
    }

    public interface IValuePort<T> : IValuePort
    {
        new T Value { get; set; }
    }

    public interface IInputValuePort<T> : IValuePort<T>, IInputValuePort { }

    public interface IOutputValuePort<T> : IValuePort<T>, IOutputValuePort { }

    #endregion Value Ports

    #region Flow Ports

    public interface IFlowPort : IPort
    {
        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<FlowConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IFlowPort other);

        /// <inheritdoc cref="IPort.Connect" />
        FlowConnection Connect(IFlowPort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IFlowPort other);

        void Flow();
    }

    public interface IInputFlowPort : IFlowPort, IInputPort
    {
        /// <summary>
        ///     Index of current port in <see cref="FlowUnit.InputFlowPorts " />.
        /// </summary>
        int InputFlowIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<FlowConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IOutputFlowPort other);

        /// <inheritdoc cref="IPort.Connect" />
        FlowConnection Connect(IOutputFlowPort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IOutputFlowPort other);
    }

    public interface IOutputFlowPort : IFlowPort, IOutputPort
    {
        /// <summary>
        ///     Index of current port in <see cref="FlowUnit.OutputFlowPorts " />.
        /// </summary>
        int OutputFlowIndex { get; }

        /// <inheritdoc cref="IPort.Connections" />
        new IEnumerable<FlowConnection> Connections { get; }

        /// <inheritdoc cref="IPort.CanConnect" />
        bool CanConnect(IInputFlowPort other);

        /// <inheritdoc cref="IPort.Connect" />
        FlowConnection Connect(IInputFlowPort other);

        /// <inheritdoc cref="IPort.Disconnect" />
        bool Disconnect(IInputFlowPort other);
    }

    #endregion Flow Ports
}