using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity.Assets
{
    public static class AssetBundles
    {
        #region Paths

        #region Root

        public const string RootDirectoryName = "Bundles";

        public const string RootFileName = RootDirectoryName;

        public static string RootDirectory => _rootDirectory.Value;

        public static string RootFilePath => _rootFilePath.Value;

        private static readonly Lazy<string> _rootDirectory = new Lazy<string>(MakeRootDirectory);
        private static readonly Lazy<string> _rootFilePath = new Lazy<string>(MakeRootFilePath);

        private static string MakeRootDirectory()
        {
            return Path.Combine(Application.streamingAssetsPath, RootDirectoryName);
        }

        private static string MakeRootFilePath() { return Path.Combine(RootDirectory, RootFileName); }

        #endregion Root

        public static string PathToName(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));
            UnityEnsure.Argument.AssetPath(path, nameof(path));

            path = PathEx.Normalize(path.RemoveHead("Assets".Length));
            if (path.StartsWith(PathEx.DirectorySeparatorChar.ToString()))
                path = path.RemoveHead(1);
            var ext = Path.GetExtension(path);
            var name = path.RemoveTail(ext.Length);
            return name.ToLowerInvariant();
        }

        public static string NameToPath(string name) { return NameToPath(name, string.Empty); }

        public static string NameToPath(string name, string baseDirectory)
        {
            Ensure.Argument.NotNullOrEmpty(name, nameof(name));

            var list = (baseDirectory ?? string.Empty).Concat(name.Split('/'));
            return PathEx.Combine(list.ToArray());
        }

        #endregion Paths

        #region Manifest

        public static AssetBundleManifest Manifest => _manifest.Value;

        private static readonly Lazy<AssetBundleManifest> _manifest = new Lazy<AssetBundleManifest>(LoadManifest);

        private const string ManifestAssetName = "AssetBundleManifest";

        private static AssetBundleManifest LoadManifest()
        {
            var rootBundle = AssetBundle.LoadFromFile(RootFilePath);
            return rootBundle.LoadAsset<AssetBundleManifest>(ManifestAssetName);
        }

        #endregion Manifest

        #region Instances

        /// <summary>
        ///     Get the asset bundle instance with specified path it is already loaded; otherwise load it from disk and
        ///     return the loaded instance.
        /// </summary>
        /// <param name="name">Path of the asset bundle to get or load.</param>
        /// <param name="loadDependencies">
        ///     Set the value to <see langword="true" /> if you want to load asset bundles that the specified asset bundle
        ///     dependent on; otherwise <see langword="false" /> to load the specified asset bundle only.
        /// </param>
        /// <returns>The asset bundle instance named <paramref name="name" />.</returns>
        public static AssetBundle GetOrLoadFromFile(string name, bool loadDependencies = true)
        {
            Ensure.Argument.NotNullOrEmpty(name, nameof(name));

            if (!TryGetOrLoadFromFile(name, out var info))
                return null;

            if (loadDependencies && !info.IsStreamedSceneAssetBundle)
            {
                foreach (var dependency in info.Dependencies)
                    TryGetOrLoadFromFile(dependency, out _);
            }

            return info.Bundle;
        }

        private static readonly Dictionary<string, BundleInfo> _loadedBundles = new Dictionary<string, BundleInfo>();

        private static bool TryGetOrLoadFromFile(string name, out BundleInfo info)
        {
            return _loadedBundles.TryGetValue(name, out info) || TryLoadFromFile(name, out info);
        }

        private static bool TryLoadFromFile(string name, out BundleInfo info)
        {
            info = default;

            var bundlePath = NameToPath(name, RootDirectory);
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            if (!bundle)
                return false;

            var dependencies = Manifest.GetAllDependencies(name);

            info = new BundleInfo(bundle, dependencies);
            _loadedBundles.Add(name, info);
            return true;
        }

        private sealed class BundleInfo : IEquatable<BundleInfo>
        {
            public readonly AssetBundle Bundle;

            public readonly IReadOnlyCollection<string> Dependencies;

            public bool IsStreamedSceneAssetBundle => Bundle.isStreamedSceneAssetBundle;

            public BundleInfo(AssetBundle bundle, IEnumerable<string> dependencies)
            {
                Bundle = bundle;
                Dependencies = new HashSet<string>(dependencies);
            }

            public bool Equals(BundleInfo other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(Bundle, other.Bundle);
            }

            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj) || obj is BundleInfo other && Equals(other);
            }

            public override int GetHashCode() { return Bundle.GetHashCode(); }
        }

        #endregion Instances
    }
}