using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.UIElements.Demo.Editor
{
    public class BuiltinUSSVariablesWindow : EditorWindow
    {
        [MenuItem(MenuInfo.Path + "Built-in USS Variables")]
        public static BuiltinUSSVariablesWindow Open()
        {
            var w = GetWindow<BuiltinUSSVariablesWindow>();
            w.Show();
            return w;
        }

        private Vector2 _scrollPosition;

        private void OnGUI()
        {
            using var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition);

            MetricsGUI();
            ColorsGUI();
            IconsGUI();

            _scrollPosition = scroll.scrollPosition;
        }

        #region Metrics

        private static bool _metricsFoldout = true;

        private static void MetricsGUI()
        {
            _metricsFoldout = EditorGUILayout.Foldout(_metricsFoldout, "Metrics");
            if (!_metricsFoldout)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var p in BuiltinUSSVariables.Metrics)
                    EditorGUILayout.IntField(new GUIContent(p.Key) { tooltip = p.Key }, p.Value);
            }
        }

        #endregion Metrics

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

        private static void ColorsGUI(ref bool foldout, string title, IReadOnlyDictionary<string, Color> colorDict)
        {
            foldout = EditorGUILayout.Foldout(foldout, title);
            if (!foldout)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var p in colorDict)
                    EditorGUILayout.ColorField(new GUIContent(p.Key) { tooltip = p.Key }, p.Value);
            }
        }

        #endregion Colors

        #region Icons

        private static bool _darkThemeIconsFoldout = true;
        private static bool _lightThemeIconsFoldout = true;

        private static void IconsGUI()
        {
            IconsGUI(ref _darkThemeIconsFoldout, "Dark Theme Icons", BuiltinUSSVariables.DarkThemeIcons);
            IconsGUI(ref _lightThemeIconsFoldout, "Light Theme Icons", BuiltinUSSVariables.LightThemeIcons);
        }

        private static void IconsGUI(ref bool foldout, string title, IReadOnlyDictionary<string, Texture2D> iconDict)
        {
            foldout = EditorGUILayout.Foldout(foldout, title);
            if (!foldout)
                return;

            using (new EditorGUI.IndentLevelScope())
            {
                foreach (var p in iconDict)
                    IconGUI(p.Key, p.Value);
            }
        }

        private static void IconGUI(string name, Texture2D icon)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var labelWidth = GUILayout.Width(EditorGUIUtility.labelWidth);
                var labelRect = EditorGUILayout.GetControlRect(labelWidth);
                EditorGUI.LabelField(labelRect, new GUIContent(name) { tooltip = name });

                GUILayoutOption iconWidth = GUILayout.Width(icon.width), iconHeight = GUILayout.Height(icon.height);
                var iconRect = EditorGUILayout.GetControlRect(iconWidth, iconHeight);
                DrawIconBackground(iconRect);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }
        }

        private static void DrawIconBackground(Rect rect)
        {
            //GUI.DrawTexture();
        }

        #endregion Icons
    }
}
