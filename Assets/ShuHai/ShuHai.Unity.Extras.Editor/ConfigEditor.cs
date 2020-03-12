using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Editor
{
    [CustomEditor(typeof(Config))]
    public class ConfigEditor : UnityEditor.Editor
    {
        public Config Target => (Config)target;

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();
//
//            using (new EditorGUILayout.HorizontalScope()) AddGUI();
//        }
//
//        protected void AddGUI()
//        {
//            if (!GUILayout.Button("Add", EditorStyles.miniButton))
//                return;
//
//            Undo.RecordObject(Target, "Config.Add");
//
//            string key = "Key", value = string.Empty;
//            var existedCount = 0;
//            while (Target.ContainsKey(key))
//                key = key + ++existedCount;
//            Target.Add(key, value);
//        }
    }
}