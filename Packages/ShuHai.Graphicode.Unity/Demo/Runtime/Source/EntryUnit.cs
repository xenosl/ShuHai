namespace ShuHai.Graphicode.Unity.Demo
{
    public class EntryUnit : FlowUnit
    {
        public IOutputFlowPort StartPort => OutputFlowPorts[0];

        public EntryUnit()
            : base("Entry")
        {
            AddOutputFlowPort("Start");
        }

        protected override void ExecuteImpl(IInputFlowPort byPort) { StartPort.Flow(); }
    }
}
