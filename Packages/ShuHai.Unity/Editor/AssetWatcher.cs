using System;
using System.Collections.Generic;
using UnityEditor;

namespace ShuHai.Unity.Editor
{
    public enum WatchType
    {
        /// <summary>
        ///     Watches asset or folder at specified path.
        /// </summary>
        SingleAsset,

        /// <summary>
        ///     Watches all assets in specified folder.
        /// </summary>
        AssetsInFolder
    }

    public sealed class AssetWatcher
    {
        #region Events

        public event Action<string> AssetImported;
        public event Action<string> AssetDeleted;
        public event Action<string, string> AssetMoved;

        private void RiseAssetImported(string path)
        {
            if (CanRiseEvent(path))
                AssetImported?.Invoke(path);
        }

        private void RiseAssetDeleted(string path)
        {
            if (CanRiseEvent(path))
                AssetDeleted?.Invoke(path);
        }

        private void RiseAssetMoved(string oldPath, string newPath)
        {
            if (CanRiseEvent(oldPath))
                AssetMoved?.Invoke(oldPath, newPath);
            if (CanRiseEvent(newPath))
                AssetMoved?.Invoke(oldPath, newPath);
        }

        private bool CanRiseEvent(string path)
        {
            switch (WatchType)
            {
                case WatchType.SingleAsset:
                    return path == Path;
                case WatchType.AssetsInFolder:
                    return path.Contains(Path) && AssetDatabase.IsValidFolder(Path);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static void OnAssetImported(string path)
        {
            foreach (var instance in _activeInstances)
                instance.RiseAssetImported(path);
        }

        internal static void OnAssetDeleted(string path)
        {
            foreach (var instance in _activeInstances)
                instance.RiseAssetDeleted(path);
        }

        internal static void OnAssetMoved(string oldPath, string newPath)
        {
            foreach (var instance in _activeInstances)
                instance.RiseAssetMoved(oldPath, newPath);
        }

        #endregion Events

        #region Activation

        public bool Active
        {
            get => _active;
            set
            {
                if (value == _active)
                    return;

                _active = value;

                if (_active)
                    _activeInstances.Add(this);
                else
                    _activeInstances.Remove(this);
            }
        }

        private bool _active;

        private static readonly HashSet<AssetWatcher> _activeInstances = new HashSet<AssetWatcher>();

        #endregion Activation

        public string Path { get; }

        public WatchType WatchType { get; }

        public AssetWatcher(string path, WatchType watchType, bool active = true)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            path = PathEx.Normalize(path);
            if (!UnityPath.IsAssetPath(path))
                throw new ArgumentException("Asset path expected.", nameof(path));

            Path = path;
            WatchType = watchType;

            Active = active;
        }
    }

    internal class AssetWatcherImpl : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
                AssetWatcher.OnAssetImported(path);
            foreach (var path in deletedAssets)
                AssetWatcher.OnAssetDeleted(path);
            for (int i = 0; i < movedAssets.Length; ++i)
                AssetWatcher.OnAssetMoved(movedFromAssetPaths[i], movedAssets[i]);
        }

        public override int GetPostprocessOrder() { return int.MaxValue; }
    }
}
