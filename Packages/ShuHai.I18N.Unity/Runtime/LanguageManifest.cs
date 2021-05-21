using System;
using System.Collections.Generic;
using ShuHai.Unity;
using UnityEngine;

namespace ShuHai.I18N.Unity
{
    [Serializable]
    public struct LanguageAsset
    {
        public string LanguageName;

        public TextAsset[] TextAssets;
    }

    [CreateAssetMenu(menuName = MenuInfo.Path + "LanguageManifest", fileName = "LanguageManifest")]
    public class LanguageManifest : ScriptableObject, ISerializationCallbackReceiver
    {
        public IReadOnlyDictionary<string, LanguageAsset> LanguageAssets => _languageAssetDict;

        private readonly Dictionary<string, LanguageAsset> _languageAssetDict = new Dictionary<string, LanguageAsset>();
#pragma warning disable 649
        [SerializeField]
        private LanguageAsset[] _languageAssets;
#pragma warning restore 649

        private void ApplyLanguageAssets()
        {
            if (CollectionUtil.IsNullOrEmpty(_languageAssets))
                return;

            _languageAssetDict.Clear();
            foreach (var asset in _languageAssets)
            {
                if (string.IsNullOrEmpty(asset.LanguageName))
                    continue;

                if (_languageAssetDict.ContainsKey(asset.LanguageName))
                {
                    Debug.LogWarning($"Duplicate language asset named '{asset.LanguageName}' found.");
                    continue;
                }
                _languageAssetDict.Add(asset.LanguageName, asset);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { ApplyLanguageAssets(); }

        #region Unity Events

#if UNITY_EDITOR
        private void OnValidate() { ApplyLanguageAssets(); }
#endif

        #endregion Unity Events
    }
}
