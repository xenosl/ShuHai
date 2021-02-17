using System.Diagnostics;

namespace ShuHai.Graphicode
{
    public sealed class BranchUnit : FlowUnit
    {
        public IInputFlowPort ExecutePort => InputFlowPorts[0];

        public IInputValuePort<bool> ConditionPort => (IInputValuePort<bool>)InputValuePorts[0];

        public IOutputFlowPort TruePort => OutputFlowPorts[0];
        public IInputFlowPort FalsePort => InputFlowPorts[0];

        public BranchUnit()
            : base("Branch")
        {
            AddInputFlowPort();
            AddInputValuePort<bool>("Condition");

            AddOutputFlowPort("True");
            AddOutputFlowPort("False");
        }

        protected override void ExecuteImpl(IInputFlowPort byPort)
        {
            Debug.Assert(byPort == ExecutePort);
            
            if (ConditionPort.Value)
                TruePort.Flow();
            else
                FalsePort.Flow();
        }
    }
}