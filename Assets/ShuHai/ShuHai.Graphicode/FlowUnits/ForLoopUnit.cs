using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public class ForLoopUnit : FlowUnit
    {
        public IInputFlowPort StartPort => InputFlowPorts[0];
        public IInputValuePort<int> FirstIndexPort => (IInputValuePort<int>)InputValuePorts[0];
        public IInputValuePort<int> LastIndexPort => (IInputValuePort<int>)InputValuePorts[1];
        public IInputFlowPort BreakPort => InputFlowPorts[1];

        public IOutputFlowPort BodyPort => OutputFlowPorts[0];
        public IOutputValuePort<int> IndexPort => (IOutputValuePort<int>)OutputValuePorts[0];
        public IOutputFlowPort CompletePort => OutputFlowPorts[1];

        public ForLoopUnit()
            : base("For Loop")
        {
            AddInputFlowPort("Start");
            AddInputValuePort<int>("First Index");
            AddInputValuePort<int>("Last Index");
            AddInputFlowPort("Break");

            AddOutputFlowPort("Loop Body");
            AddOutputValuePort<int>("Body Index");
            AddOutputFlowPort("Complete");
        }

        protected override void ExecuteImpl(IInputFlowPort byPort)
        {
            if (byPort == StartPort)
            {
                for (int i = FirstIndexPort.Value; i <= LastIndexPort.Value; ++i)
                {
                    IndexPort.Value = i;
                    BodyPort.Flow();

                    if (needBreak)
                    {
                        needBreak = false;
                        break;
                    }
                }
                CompletePort.Flow();
            }
            else if (byPort == BreakPort)
            {
                needBreak = true;
            }
            else
            {
                Debug.Assert(false, "Undefined execution.");
            }
        }

        private bool needBreak;
    }
}