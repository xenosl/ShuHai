namespace ShuHai.Graphicode.Unity.Units
{
    internal class MonoBehaviourUnit : FlowUnit
    {
        public IOutputFlowPort AwakePort => OutputFlowPorts[0];
        public IOutputFlowPort OnDestroyPort => OutputFlowPorts[1];
        public IOutputFlowPort StartPort => OutputFlowPorts[2];
        public IOutputFlowPort OnEnablePort => OutputFlowPorts[3];
        public IOutputFlowPort OnDisablePort => OutputFlowPorts[4];
        public IOutputFlowPort UpdatePort => OutputFlowPorts[5];
        public IOutputFlowPort LateUpdatePort => OutputFlowPorts[6];
        public IOutputFlowPort OnApplicationQuit => OutputFlowPorts[7];

        public MonoBehaviourUnit() : this("MonoBehaviour") { }

        public MonoBehaviourUnit(string name)
            : base(name)
        {
            AddOutputFlowPort("Awake");
            AddOutputFlowPort("OnDestroy");
            AddOutputFlowPort("Start");
            AddOutputFlowPort("OnEnable");
            AddOutputFlowPort("OnDisable");
            AddOutputFlowPort("Update");
            AddOutputFlowPort("LateUpdate");
            AddOutputFlowPort("OnApplicationQuit");
        }

        protected override void ExecuteImpl(IInputFlowPort byPort) { }
    }
}