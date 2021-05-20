using System.Collections.Generic;
using ShuHai.Unity.IMGUI;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.UIElements.Demo.Editor
{
    [CustomEditor(typeof(BuiltinUSSVariablesObject))]
    public class BuiltinUSSVariablesObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MetricsGUI();
            ColorsGUI();
        }

        private static bool _metricsFoldout = true;

        private static void MetricsGUI()
        {
            _metricsFoldout = EditorGUILayout.Foldout(_metricsFoldout, "Metrics");
            if (!_metricsFoldout)
                return;

            using (new GUIEnabledScope(false))
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var p in BuiltinUSSVariables.Metrics)
                    EditorGUILayout.IntField(new GUIContent(p.Key) { tooltip = p.Key }, p.Value);
            }
        }

        #region Colors

        private static bool _runtimeColorsFoldout = true;
        private static bool _darkThemeColorsFoldout = true;
        private static bool _lightThemeColorsFoldout = true;

        private static void ColorsGUI()
        {
            ColorsGUI(ref _runtimeColorsFoldout, "Runtime Colors", BuiltinUSSVariables.RuntimeColors);
            ColorsGUI(ref _darkThemeColorsFoldout, "Dark Theme Colors", BuiltinUSSVariables.DarkThemeColors);
            ColorsGUI(ref _lightThemeColorsFoldout, "Light Theme Colors", BuiltinUSSVariables.LightThemeColors);
        }

        private static void ColorsGUI(ref bool foldout, string name, IReadOnlyDictionary<string, Color> colorDict)
        {
            foldout = EditorGUILayout.Foldout(foldout, name);
            if (!foldout)
                return;

            using (new GUIEnabledScope(false))
            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var p in colorDict)
                    EditorGUILayout.ColorField(new GUIContent(p.Key) { tooltip = p.Key }, p.Value);
            }
        }

        #endregion Colors
    }
}
