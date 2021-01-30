using System;

namespace ShuHai.Graphicode
{
    public sealed class FlowConnection : Connection
    {
        public new IOutputFlowPort Source => (IOutputFlowPort)base.Source;

        public new IInputFlowPort Destination => (IInputFlowPort)base.Destination;

        internal FlowConnection(IOutputFlowPort source, IInputFlowPort destination) : base(source, destination) { }

        /// <summary>
        ///     Get a value indicates whether the specified ports can be connected.
        /// </summary>
        /// <param name="source">The source port of the desired connection.</param>
        /// <param name="destination">The destination port of the desired connection.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified ports can be connected; otherwise, <see langword="false" />.
        /// </returns>
        public static bool CanConnect(IOutputFlowPort source, IInputFlowPort destination)
        {
            return CanConnectImpl(source, destination);
        }

        internal static bool CanConnectImpl(
            IOutputFlowPort source, IInputFlowPort destination, bool checkExistence = true)
        {
            if (checkExistence && !Exist(source, destination))
                return false;
            return true;
        }

        internal static FlowConnection ConnectImpl(
            IOutputFlowPort source, IInputFlowPort destination, Action<FlowConnection> postAction)
        {
            return ConnectImpl((src, dst) => new FlowConnection(src, dst), source, destination, postAction);
        }

        internal static bool DisconnectImpl(
            IOutputFlowPort source, IInputFlowPort destination, Action<FlowConnection> preAction)
        {
            return Connection.DisconnectImpl(source, destination, preAction);
        }
    }
}