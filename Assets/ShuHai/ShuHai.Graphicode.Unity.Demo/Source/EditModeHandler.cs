using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    public class EditModeHandler : ModeHandler
    {
        public override DemoMode Mode { get; } = DemoMode.Edit;

        #region Graphs

        private void EnableGraphPanels() { Demo.LevelGraphPanel.Active = true; }

        private void DisableGraphPanels() { Demo.LevelGraphPanel.Active = false; }

        #endregion Graphs

        #region Unity Events

        protected override void OnEnable()
        {
            base.OnEnable();
            EnableGraphPanels();
        }

        protected override void OnDisable()
        {
            DisableGraphPanels();
            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Return))
                Demo.Mode = DemoMode.Play;
        }

        #endregion Unity Events
    }
}