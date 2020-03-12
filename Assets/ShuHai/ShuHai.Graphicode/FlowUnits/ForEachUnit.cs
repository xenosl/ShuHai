using System.Collections.Generic;
using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public class ForEachUnit<T> : FlowUnit
    {
        public IInputFlowPort StartPort => InputFlowPorts[0];
        public IInputValuePort<IEnumerable<T>> CollectionPort => (IInputValuePort<IEnumerable<T>>)InputValuePorts[0];
        public IInputFlowPort BreakPort => InputFlowPorts[1];

        public IOutputFlowPort BodyPort => OutputFlowPorts[0];
        public IOutputValuePort<int> IndexPort => (IOutputValuePort<int>)OutputValuePorts[0];
        public IOutputValuePort<T> ItemPort => (IOutputValuePort<T>)OutputValuePorts[1];
        public IOutputFlowPort CompletePort => OutputFlowPorts[1];

        public ForEachUnit()
            : base("For Each")
        {
            AddInputFlowPort("Start");
            AddInputValuePort<IEnumerable<T>>("Collection");
            AddInputFlowPort("Break");

            AddOutputFlowPort("Body");
            AddOutputValuePort<int>("Index");
            AddOutputValuePort<T>("Item");
            AddOutputFlowPort("Complete");
        }

        protected override void ExecuteImpl(IInputFlowPort byPort)
        {
            if (byPort == StartPort)
            {
                IndexPort.Value = 0;
                foreach (var item in CollectionPort.Value)
                {
                    ItemPort.Value = item;
                    BodyPort.Flow();

                    if (_needBreak)
                    {
                        _needBreak = false;
                        break;
                    }

                    ++IndexPort.Value;
                }
                CompletePort.Flow();
            }
            else if (byPort == BreakPort)
            {
                _needBreak = true;
            }
            else
            {
                Debug.Assert(false, "Undefined execution.");
            }
        }

        private bool _needBreak;
    }
}