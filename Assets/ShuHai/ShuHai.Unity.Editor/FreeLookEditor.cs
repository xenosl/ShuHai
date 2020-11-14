using UnityEditor;

namespace ShuHai.Unity.Editor
{
    [CustomEditor(typeof(FreeLook))]
    public class FreeLookEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            InputSystemWarningGUI();
            base.OnInspectorGUI();
        }

        private static void InputSystemWarningGUI()
        {
#if ENABLE_INPUT_SYSTEM
            EditorGUILayout.HelpBox(
                "The component is only available when using legacy input mechanism (UnityEngine.Input).",
                MessageType.Error);
#endif
        }
    }
}