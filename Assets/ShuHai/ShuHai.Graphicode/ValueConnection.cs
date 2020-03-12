using System;

namespace ShuHai.Graphicode
{
    public sealed class ValueConnection : Connection
    {
        public new IOutputValuePort Source => (IOutputValuePort)base.Source;

        public new IInputValuePort Destination => (IInputValuePort)base.Destination;

        internal ValueConnection(IOutputValuePort source, IInputValuePort destination) : base(source, destination) { }

        /// <summary>
        ///     Get a value indicates whether the specified ports can be connected.
        ///     This method is equivalent to <see cref="IInputFlowPort.CanConnect(IOutputFlowPort)" /> and
        ///     <see cref="IOutputFlowPort.CanConnect(IInputFlowPort)" />.
        /// </summary>
        /// <param name="source">The source port of the desired connection.</param>
        /// <param name="destination">The destination port of the desired connection.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified ports can be connected; otherwise, <see langword="false" />.
        /// </returns>
        public static bool CanConnect(IOutputValuePort source, IInputValuePort destination)
        {
            return CanConnectImpl(source, destination);
        }

        internal static bool CanConnectImpl(
            IOutputValuePort source, IInputValuePort destination, bool checkExistence = true)
        {
            if (checkExistence && !Exist(source, destination))
                return false;
            return destination.ValueType.IsAssignableFrom(source.ValueType);
        }

        internal static ValueConnection ConnectImpl(
            IOutputValuePort source, IInputValuePort destination, Action<ValueConnection> postAction)
        {
            return ConnectImpl((src, dst) => new ValueConnection(src, dst), source, destination, postAction);
        }

        internal static bool DisconnectImpl(
            IOutputValuePort source, IInputValuePort destination, Action<ValueConnection> preAction)
        {
            return Connection.DisconnectImpl(source, destination, preAction);
        }
    }
}
