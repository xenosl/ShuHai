using ShuHai.Unity;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    public class PlayModeHandler : ModeHandler
    {
        public override DemoMode Mode { get; } = DemoMode.Play;

        private void HideGameObjectGraphPanels()
        {
            foreach (var (_, inst) in GameObjectGraph.Instances)
                inst.PanelActive = false;
        }
        
        #region Unity Events

        protected override void OnEnable()
        {
            base.OnEnable();

            HideGameObjectGraphPanels();

            Player.Instance.GetComponent<FreeLook>().enabled = true;
        }

        protected override void OnDisable()
        {
            Player.Instance.GetComponent<FreeLook>().enabled = false;

            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyUp(KeyCode.Return))
                Demo.Mode = DemoMode.Edit;
        }

        #endregion Unity Events
    }
}