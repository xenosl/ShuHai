using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Assets
{
    public sealed class AssetManifest : ScriptableObject
        , ISerializationCallbackReceiver, IDictionary<string, AssetManifest.AssetInfo>
    {
        #region Creation

        public static AssetManifest Create() { return CreateInstance<AssetManifest>(); }

        public static AssetManifest FromJson(string json)
        {
            var obj = Create();
            JsonUtility.FromJsonOverwrite(json, obj);
            return obj;
        }

        #endregion Creation

        [Serializable]
        public struct AssetInfo
        {
            /// <summary>
            ///     Path of the asset current instance represents.
            /// </summary>
            public string Path;

            public string BundleName;

            /// <summary>
            ///     Path list of all assets the current asset depend on.
            /// </summary>
            public string[] Dependencies;
        }

        private AssetManifest() { }

        public ICollection<string> Keys => _dict.Keys;

        public ICollection<AssetInfo> Values => _dict.Values;

        public int Count => _dict.Count;

        public AssetInfo this[string key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }

        public bool Add(string assetPath, string bundleName, string[] dependencies)
        {
            return Add(new AssetInfo
            {
                Path = assetPath,
                BundleName = bundleName,
                Dependencies = dependencies
            });
        }

        public bool Add(AssetInfo asset)
        {
            var path = asset.Path;
            if (_dict.ContainsKey(path))
                return false;

            _dict.Add(path, asset);
            return true;
        }

        public int Add(IEnumerable<AssetInfo> assets) { return assets.Count(Add); }

        public int Add(AssetManifest manifest) { return Add(manifest.Values); }

        public bool Remove(string key) { return _dict.Remove(key); }

        public int Remove(IEnumerable<string> assetPaths) { return assetPaths.Count(Remove); }

        public void Clear() { _dict.Clear(); }

        public bool ContainsKey(string key) { return _dict.ContainsKey(key); }

        public bool TryGetValue(string key, out AssetInfo value) { return _dict.TryGetValue(key, out value); }

        public IEnumerator<KeyValuePair<string, AssetInfo>> GetEnumerator() { return _dict.GetEnumerator(); }

        void IDictionary<string, AssetInfo>.Add(string path, AssetInfo value)
        {
            if (path != value.Path)
                throw new ArgumentException("AssetInfo.Path does not match the path argument.");
            Add(value);
        }

        private Dictionary<string, AssetInfo> _dict = new Dictionary<string, AssetInfo>();

        [SerializeField]
        private AssetInfo[] _assets;

        #region ICollection

        private ICollection<KeyValuePair<string, AssetInfo>> Collection => _dict;

        bool ICollection<KeyValuePair<string, AssetInfo>>.IsReadOnly => Collection.IsReadOnly;

        void ICollection<KeyValuePair<string, AssetInfo>>.Add(KeyValuePair<string, AssetInfo> item)
        {
            Collection.Add(item);
        }

        bool ICollection<KeyValuePair<string, AssetInfo>>.Remove(KeyValuePair<string, AssetInfo> item)
        {
            return Collection.Remove(item);
        }

        bool ICollection<KeyValuePair<string, AssetInfo>>.Contains(KeyValuePair<string, AssetInfo> item)
        {
            return Collection.Contains(item);
        }

        void ICollection<KeyValuePair<string, AssetInfo>>.CopyTo(
            KeyValuePair<string, AssetInfo>[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        #endregion ICollection

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_dict).GetEnumerator(); }

        #endregion IEnumerable

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { _assets = _dict.Select(p => p.Value).ToArray(); }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { _dict = _assets.ToDictionary(a => a.Path); }

        #endregion ISerializationCallbackReceiver
    }
}