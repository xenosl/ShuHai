using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.I18N.Unity.Editor
{
    [CustomEditor(typeof(LanguageManifest))]
    public class LanguageManifestEditor : UnityEditor.Editor
    {
        public LanguageManifest Target => (LanguageManifest)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LanguageAssetsGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable() { EnableLanguageAssets(); }
        private void OnDisable() { DisableLanguageAssets(); }

        #region Language Assets

        private SerializedProperty _languageAssets;

        private void EnableLanguageAssets() { _languageAssets = serializedObject.EnsureProperty("_languageAssets"); }
        private void DisableLanguageAssets() { _languageAssets = null; }

        private void LanguageAssetsGUI()
        {
            if (_languageAssets == null)
                return;

            if (GUILayout.Button("Add Language Folder"))
            {
                var dir = EditorUtility.OpenFolderPanel("Select Language Folder", Project.AssetsPath, null);
                AddLanguageAssetFolder(dir);
            }
        }

        private void AddLanguageAssetFolder(string path)
        {
            var files = UnityPath.EnumerateFiles(path, "*.json");
            var asset = new LanguageAsset
            {
                LanguageName = Path.GetFileNameWithoutExtension(path),
                TextAssets = files.Select(AssetDatabase.LoadAssetAtPath<TextAsset>).ToArray()
            };

            var array = (LanguageAsset[])_languageAssets.GetValue();
            var set = new HashSet<LanguageAsset>(array) { asset };
            _languageAssets.SetValue(set.ToArray());
        }

        #endregion Language Assets
    }
}
