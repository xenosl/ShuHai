using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public struct ConnectionKey : IEquatable<ConnectionKey>
    {
        public readonly IOutputPort Source;
        public readonly IInputPort Destination;

        public ConnectionKey(IOutputPort source, IInputPort destination)
        {
            Source = source;
            Destination = destination;
        }

        public bool Equals(ConnectionKey other)
        {
            return Equals(Source, other.Source) && Equals(Destination, other.Destination);
        }

        public override bool Equals(object obj) { return obj is ConnectionKey other && Equals(other); }

        public override int GetHashCode() { return HashCode.Get(Source, Destination); }

        public static bool operator ==(ConnectionKey l, ConnectionKey r) { return l.Equals(r); }
        public static bool operator !=(ConnectionKey l, ConnectionKey r) { return !(l == r); }
    }

    public abstract class Connection : GraphElement, IEquatable<Connection>
    {
        public IOutputPort Source => Key.Source;
        public IInputPort Destination => Key.Destination;

        public readonly ConnectionKey Key;

        /// <summary>
        ///     Indicates whether the current connection is a valid connection which connect two ports.
        ///     A connection became invalid once disconnected.
        /// </summary>
        public bool IsValid { get; private set; }

        protected Connection(IOutputPort source, IInputPort destination)
            : this(new ConnectionKey(source, destination)) { }

        protected Connection(ConnectionKey key)
        {
            Key = key;
            _hashCode = key.GetHashCode();
        }

        #region Comparison

        public bool Equals(Connection other) { return other != null && Key == other.Key; }

        public override bool Equals(object obj) { return obj is Connection other && Equals(other); }

        public override int GetHashCode() { return _hashCode; }

        private readonly int _hashCode;

        public static bool operator ==(Connection l, Connection r)
        {
            return EqualityComparer<Connection>.Default.Equals(l, r);
        }

        public static bool operator !=(Connection l, Connection r) { return !(l == r); }

        #endregion Comparison

        #region Management

        public static Connection Get(IOutputPort source, IInputPort destination)
        {
            return _instances.GetValue(new ConnectionKey(source, destination));
        }

        public static bool Exist(IOutputPort source, IInputPort destination)
        {
            return Exist(new ConnectionKey(source, destination));
        }

        public static bool Exist(ConnectionKey key) { return _instances.ContainsKey(key); }

        private static readonly Dictionary<ConnectionKey, Connection>
            _instances = new Dictionary<ConnectionKey, Connection>();

        #region Create

        public static bool CanConnect(IOutputPort source, IInputPort destination)
        {
            if (source is IOutputFlowPort flowSrc && destination is IInputFlowPort flowDst)
                return FlowConnection.CanConnect(flowSrc, flowDst);
            if (source is IOutputValuePort valueSrc && destination is IInputValuePort valueDst)
                return ValueConnection.CanConnect(valueSrc, valueDst);
            return false;
        }

        /// <summary>
        ///     Connect the specified ports.
        ///     Equivalent to <see cref="IOutputPort.Connect(IInputPort)" /> and <see cref="IInputPort.Connect(IOutputPort)" />.
        /// </summary>
        /// <param name="source">The source port of the connection.</param>
        /// <param name="destination">The destination port of the connection.</param>
        /// <returns>
        ///     A <see cref="Connection" /> object represents the newly created connection between the specified ports
        ///     if the two ports can be connected and the two ports are not connected, or the existed connection between
        ///     the two ports if they already connected; otherwise, <see langword="null" /> if the specified ports are
        ///     unable to connect together.
        /// </returns>
        public static Connection Connect(IPort source, IPort destination)
        {
            if (source is IOutputFlowPort flowSrc && destination is IInputFlowPort flowDst)
                return flowSrc.Connect(flowDst);
            if (source is IOutputValuePort valueSrc && destination is IInputValuePort valueDst)
                return valueSrc.Connect(valueDst);
            return null;
        }

        protected internal static bool CanConnectImpl(
            IOutputPort source, IInputPort destination, bool checkExistence = true)
        {
            if (source is IOutputFlowPort flowSrc && destination is IInputFlowPort flowDst)
                return FlowConnection.CanConnectImpl(flowSrc, flowDst, checkExistence);
            if (source is IOutputValuePort valueSrc && destination is IInputValuePort valueDst)
                return ValueConnection.CanConnectImpl(valueSrc, valueDst, checkExistence);
            return false;
        }

        protected internal static TConnection ConnectImpl<TConnection, TOutputPort, TInputPort>(
            Func<TOutputPort, TInputPort, TConnection> factory,
            TOutputPort source, TInputPort destination, Action<TConnection> postAction)
            where TConnection : Connection
            where TOutputPort : IOutputPort
            where TInputPort : IInputPort
        {
            var conn = (TConnection)Get(source, destination);
            if (conn != null)
                return conn;

            if (!CanConnectImpl(source, destination, false))
                return null;

            conn = factory(source, destination);
            _instances.Add(conn.Key, conn);
            postAction?.Invoke(conn);

            conn.AddToGraphIfNecessary();
            conn.IsValid = true;

            return conn;
        }

        private void AddToGraphIfNecessary()
        {
            Debug.Assert(Owner == null); // The newly created connection isn't belong to any graph.

            Graph srcGraph = Source.Owner.Owner, dstGraph = Destination.Owner.Owner;
            if (srcGraph != null && dstGraph != null && srcGraph == dstGraph)
                srcGraph.AddElement(this);
        }

        protected override void OnAddedToGraph()
        {
            base.OnAddedToGraph();
            IsValid = true;
        }

        #endregion Create

        #region Destroy

        public void Disconnect()
        {
            if (IsValid)
                Source.Disconnect(Destination);
        }

        protected internal static bool DisconnectImpl<T>(
            IOutputPort source, IInputPort destination, Action<T> preAction)
            where T : Connection
        {
            var conn = (T)Get(source, destination);
            if (conn == null)
                return false;

            Debug.Assert(conn.IsValid);
            conn.IsValid = false;
            conn.RemoveFromGraphIfPossible();

            preAction?.Invoke(conn);
            _instances.Remove(conn.Key);

            return true;
        }

        private void RemoveFromGraphIfPossible() { Owner?.RemoveElement(this); }

        #endregion Destroy

        #endregion Management
    }
}