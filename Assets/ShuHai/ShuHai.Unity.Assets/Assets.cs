using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Assets
{
    using UObject = UnityEngine.Object;

    public static class Assets
    {
        #region Instantiates

        public static GameObject InstantiateGameObject(string path,
            Transform parent = null, bool worldSpace = false, bool? active = null)
        {
            var asset = GetOrLoadGameObjectForInstantiate(path, active);
            var obj = UObject.Instantiate(asset, parent, worldSpace);
            return obj;
        }

        public static GameObject InstantiateGameObject(string path,
            Vector3 position, Quaternion rotation, Transform parent = null, bool? active = null)
        {
            var asset = GetOrLoadGameObjectForInstantiate(path, active);
            var obj = UObject.Instantiate(asset, position, rotation, parent);
            return obj;
        }

        private static GameObject GetOrLoadGameObjectForInstantiate(string path, bool? active)
        {
            var asset = GetOrLoadGameObject(path);
            if (!asset)
                throw new AssetLoadException(path);

            if (active.HasValue)
                asset.SetActive(active.Value);

            return asset;
        }

        #endregion Instantiates

        #region Caches

#if UNITY_EDITOR
        public enum DataSource
        {
            AssetBundle,
            AssetDatabase
        }

        public static DataSource DataSourceInEditor = DataSource.AssetDatabase;
#endif

        public static GameObject GetOrLoadGameObject(string path) { return GetOrLoad<GameObject>(path); }

        public static T GetOrLoad<T>(string path)
            where T : UObject
        {
            return (T)GetOrLoadImpl(typeof(T), path);
        }

        public static UObject GetOrLoad(Type type, string path)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<UObject>(type, nameof(type));
            Ensure.Argument.NotNullOrEmpty(path, nameof(path));

            return GetOrLoadImpl(type, path);
        }

        private static readonly Dictionary<string, UObject> _loadedAssets = new Dictionary<string, UObject>();

        private static UObject GetOrLoadImpl(Type type, string path)
        {
            if (_loadedAssets.TryGetValue(path, out var asset))
                return asset;

            asset = Load(type, path);
            if (asset)
                _loadedAssets.Add(path, asset);

            return asset;
        }

        private static UObject Load(Type type, string path)
        {
#if UNITY_EDITOR
            switch (DataSourceInEditor)
            {
                case DataSource.AssetBundle:
                    return LoadFromAssetBundle(type, path);
                case DataSource.AssetDatabase:
                    return AssetDatabase.LoadAssetAtPath(path, type);
                default:
                    throw new EnumOutOfRangeException(DataSourceInEditor);
            }
#else
            return LoadFromAssetBundle(type, path);
#endif
        }

        private static UObject LoadFromAssetBundle(Type type, string path)
        {
            if (!Manifest.TryGetValue(path, out var assetInfo))
                return null;

            Debug.Assert(path == assetInfo.Path);
            var ab = AssetBundles.GetOrLoadFromFile(assetInfo.BundleName);
            if (!ab)
                throw new AssetLoadException(path, $"Asset bundle '{assetInfo.BundleName}' not found.");

            return ab.LoadAsset(path, type);
        }

        #endregion Caches

        #region Manifest

        public static AssetManifest Manifest => _manifest.Value;

        private static readonly Lazy<AssetManifest> _manifest = new Lazy<AssetManifest>(LoadDefaultManifest);

        private static AssetManifest LoadDefaultManifest()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "Manifest.json");
            var json = File.ReadAllText(path);
            var manifest = AssetManifest.FromJson(json);
            if (!manifest)
                manifest = AssetManifest.Create();
            return manifest;
        }

        #endregion Manifest

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (DataSourceInEditor == DataSource.AssetBundle)
                LoadDefaultManifest();
        }
    }
}