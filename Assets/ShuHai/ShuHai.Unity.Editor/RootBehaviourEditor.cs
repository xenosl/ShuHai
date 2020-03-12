using System.Collections.Generic;
using UnityEditor;

namespace ShuHai.Unity.Editor
{
    [CustomEditor(typeof(RootBehaviour))]
    public class RootBehaviourEditor : UnityEditor.Editor
    {
        private void OnEnable() { InitializeComponents(); }

        private void OnDisable() { DeinitializeComponents(); }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ComponentsGUI();
        }

        #region Components

        private readonly List<RootComponentEditor> _componentEditors = new List<RootComponentEditor>();

        private void InitializeComponents()
        {
            foreach (var component in Root.Components)
            {
                var editor = RootComponentEditor.Create(component);
                if (editor != null)
                    _componentEditors.Add(editor);
            }
        }

        private void DeinitializeComponents() { _componentEditors.Clear(); }

        private void ComponentsGUI()
        {
            foreach (var editor in _componentEditors)
                editor.GUI();
        }

        #endregion Components
    }
}