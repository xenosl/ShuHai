using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Editor
{
    [CustomEditor(typeof(RootBehaviour))]
    public class RootBehaviourEditor : UnityEditor.Editor
    {
        public MonoBehaviour Target => (MonoBehaviour)target;

        private void OnEnable() { ScriptEnable(); }

        private void OnDisable() { ScriptDisable(); }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ExecutionOrderGUI();
        }

        #region Script

        public MonoScript Script { get; private set; }

        private void ScriptEnable() { Script = MonoScript.FromMonoBehaviour(Target); }

        private void ScriptDisable() { Script = null; }

        private void ExecutionOrderGUI()
        {
            //var order = MonoImporter.GetExecutionOrder(Script);
        }

        #endregion Script
    }
}
