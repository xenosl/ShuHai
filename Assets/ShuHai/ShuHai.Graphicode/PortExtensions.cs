using System.Collections.Generic;
using System.Linq;

namespace ShuHai.Graphicode
{
    public static class PortExtensions
    {
        public static IEnumerable<IPort> GetConnectedPorts(this IPort self)
        {
            return self.Connections.Select(c => c.Source == self ? (IPort)c.Destination : c.Source);
        }

        public static IEnumerable<IInputPort> GetConnectedPorts(this IOutputPort self)
        {
            return self.Connections.Select(c => c.Destination);
        }

        public static IEnumerable<IOutputPort> GetConnectedPorts(this IInputPort self)
        {
            return self.Connections.Select(c => c.Source);
        }

        public static IEnumerable<IValuePort> GetConnectedPorts(this IValuePort self)
        {
            return self.Connections.Select(c => c.Source == self ? (IValuePort)c.Destination : c.Source);
        }

        public static IEnumerable<IInputValuePort> GetConnectedPorts(this IOutputValuePort self)
        {
            return self.Connections.Select(c => c.Destination);
        }

        public static IEnumerable<IOutputValuePort> GetConnectedPorts(this IInputValuePort self)
        {
            return self.Connections.Select(c => c.Source);
        }

        public static IEnumerable<IFlowPort> GetConnectedPorts(this IFlowPort self)
        {
            return self.Connections.Select(c => c.Source == self ? (IFlowPort)c.Destination : c.Source);
        }

        public static IEnumerable<IInputFlowPort> GetConnectedPorts(this IOutputFlowPort self)
        {
            return self.Connections.Select(c => c.Destination);
        }

        public static IEnumerable<IOutputFlowPort> GetConnectedPorts(this IInputFlowPort self)
        {
            return self.Connections.Select(c => c.Source);
        }
    }
}