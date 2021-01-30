using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Editor
{
    public sealed class AssemblyDefinition
    {
        public string Name
        {
            get => _data.name;
            set => _data.name = value;
        }

        public List<string> References => _data.references;
        public List<string> OptionalUnityReferences => _data.optionalUnityReferences;

        public List<string> IncludePlatforms => _data.includePlatforms;
        public List<string> ExcludePlatforms => _data.excludePlatforms;

        public bool AllowUnsafeCode
        {
            get => _data.allowUnsafeCode;
            set => _data.allowUnsafeCode = value;
        }

        public bool OverrideReferences
        {
            get => _data.overrideReferences;
            set => _data.overrideReferences = value;
        }

        public List<string> PrecompiledReferences => _data.precompiledReferences;

        public bool AutoReferenced
        {
            get => _data.autoReferenced;
            set => _data.autoReferenced = value;
        }

        public List<string> DefineConstraints => _data.defineConstraints;

        private readonly AssemblyDefinitionData _data;

        #region Constructors

        public AssemblyDefinition() : this(new AssemblyDefinitionData()) { }

        internal AssemblyDefinition(AssemblyDefinitionData data) { _data = data; }

        #endregion Constructors

        #region Persistence

        // TODO: The method is never tested.
        public void Save(string assetPath)
        {
            var asset = new TextAsset(_data.ToJson());
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        // TODO: The method is never tested.
        public static AssemblyDefinition Load(string assetPath)
        {
            Ensure.Argument.NotNull(assetPath, nameof(assetPath));
            if (!UnityPath.IsAssetPath(assetPath))
                throw new ArgumentException("Asset path is required.", nameof(assetPath));

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            return asset != null ? new AssemblyDefinition(AssemblyDefinitionData.FromJson(asset.text)) : null;
        }

        #endregion Persistence
    }

    [Serializable]
    internal sealed class AssemblyDefinitionData
    {
        public string name;
        public List<string> references = new List<string>();
        public List<string> optionalUnityReferences = new List<string>();
        public List<string> includePlatforms = new List<string>();
        public List<string> excludePlatforms = new List<string>();
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public List<string> precompiledReferences = new List<string>();
        public bool autoReferenced = true;
        public List<string> defineConstraints = new List<string>();

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(name))
                return false;
            if (!CollectionUtil.IsNullOrEmpty(excludePlatforms) && !CollectionUtil.IsNullOrEmpty(includePlatforms))
                return false;
            return true;
        }

        public string ToJson() { return JsonUtility.ToJson(this, true); }

        public static AssemblyDefinitionData FromJson(string json)
        {
            var assemblyData = new AssemblyDefinitionData();
            JsonUtility.FromJsonOverwrite(json, assemblyData);

            if (!assemblyData.IsValid())
                throw new Exception("Invalid AssemblyDefinition.");

            return assemblyData;
        }
    }
}
