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

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (CollectionUtil.IsNullOrEmpty(_languageAssets))
                return;

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
    }
}
