using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.UI.Widgets.Editor
{
    [CustomEditor(typeof(Root))]
    public class RootEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PrefabsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable() { EnablePrefabs(); }
        private void OnDisable() { DisablePrefabs(); }

        #region Prefabs

        private SerializedProperty _prefabs;

        private void EnablePrefabs() { _prefabs = serializedObject.EnsureProperty("_prefabs"); }
        private void DisablePrefabs() { _prefabs = null; }

        private void PrefabsGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var dir = GetDirectoryForAddingPrefab();

                if (GUILayout.Button("Add Prefab"))
                {
                    var path = EditorUtility.OpenFilePanel("Select Prefabs", dir, "prefab");
                    if (!string.IsNullOrEmpty(path))
                        AddPrefab(UnityPath.ToAsset(path));
                }

                if (GUILayout.Button("Add Prefabs In Folder"))
                {
                    dir = EditorUtility.OpenFolderPanel("Select Folder", dir, null);
                    if (!string.IsNullOrEmpty(dir))
                    {
                        foreach (var path in UnityPath.EnumerateFiles(dir, "*.prefab"))
                            AddPrefab(path);
                    }
                }
            }
        }

        private void AddPrefab(string path)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<Widget>(path);
            if (!prefab)
                return;

            var list = (List<Widget>)_prefabs.GetValue();
            list.Add(prefab);
            _prefabs.SetValue(list.Distinct().ToList());

            SetLastAddedPrefabDirectory(Path.GetDirectoryName(path));
        }

        private static readonly string _lastAddedPrefabDirectoryKey =
            typeof(Root).FullName + ".LastAddedPrefabDirectory";

        private static string GetLastAddedPrefabDirectory()
        {
            return EditorPrefs.GetString(_lastAddedPrefabDirectoryKey);
        }

        private static void SetLastAddedPrefabDirectory(string value)
        {
            EditorPrefs.SetString(_lastAddedPrefabDirectoryKey, value);
        }

        private static string GetDirectoryForAddingPrefab()
        {
            var dir = GetLastAddedPrefabDirectory();
            return string.IsNullOrEmpty(dir) ? Project.AssetsPath : dir;
        }

        #endregion Prefabs
    }
}
