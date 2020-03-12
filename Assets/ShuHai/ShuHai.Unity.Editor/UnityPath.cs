﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ShuHai.Unity.Editor
{
    public sealed class UnityPath : IEquatable<UnityPath>
    {
        public string Asset => _assetPath.Value;

        public string Rooted => _rootedPath.Value;

        public string Extension => _extension.Value;

        /// <summary>
        ///     Indicates whether the current instance represents a valid path refers to an asset in Unity project.
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(Asset);

        #region Construct

        public UnityPath(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));

            _path = PathEx.Normalize(path);

            _assetPath = new Lazy<string>(ParseAssetPath);
            _rootedPath = new Lazy<string>(ParseRootedPath);
            _extension = new Lazy<string>(() => Path.GetExtension(path));
        }

        private readonly Lazy<string> _assetPath;
        private readonly Lazy<string> _rootedPath;
        private readonly Lazy<string> _extension;
        private readonly string _path;

        private string ParseAssetPath()
        {
            if (IsAssetPath(_path))
                return _path;

            try
            {
                return ToAsset(_path);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string ParseRootedPath()
        {
            if (IsAssetPath(_path))
                return ToRooted(_path);

            return Path.IsPathRooted(_path) ? _path : string.Empty;
        }

        #endregion Construct

        public override string ToString() { return IsValid ? Asset : _path; }

        public static implicit operator UnityPath(string s) { return new UnityPath(s); }

        public static UnityPath operator +(UnityPath l, string r) { return new UnityPath(l._path + r); }

        #region Equality

        public bool Equals(UnityPath other) { return !ReferenceEquals(other, null) && _path == other._path; }

        public override bool Equals(object obj) { return obj is UnityPath other && Equals(other); }

        public override int GetHashCode() { return _path.GetHashCode(); }

        public static bool operator ==(UnityPath l, UnityPath r)
        {
            return EqualityComparer<UnityPath>.Default.Equals(l, r);
        }

        public static bool operator !=(UnityPath l, UnityPath r) { return !(l == r); }

        #endregion Equality

        #region Utilities

        public static bool IsAssetPath(string path) { return path.StartsWith("Assets"); }

        /// <summary>
        ///     Convert an asset path to rooted path of current project.
        /// </summary>
        /// <param name="path">Asset path to convert.</param>
        /// <returns>Equivalent rooted path to <paramref name="path" />.</returns>
        /// <remarks>
        ///     This method does not check whether the argument <paramref name="path" /> is a valid asset path, so if
        ///     it is not valid then the result path is invalid as well.
        /// </remarks>
        public static string ToRooted(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));
            if (!IsAssetPath(path))
                throw new ArgumentException("Asset path is required.", nameof(path));

            return PathEx.Normalize(Path.Combine(Project.RootPath, path));
        }

        /// <summary>
        ///     Convert specified rooted path which refers to an assets path to asset path for Unity.
        /// </summary>
        /// <param name="path">Rooted path to convert.</param>
        /// <returns>Asset path equivalent to <paramref name="path" />.</returns>
        public static string ToAsset(string path)
        {
            Ensure.Argument.NotNull(path, nameof(path));
            if (!Path.IsPathRooted(path))
                throw new ArgumentException("Rooted path required.", nameof(path));

            path = PathEx.Normalize(path);
            var assetsIndex = path.StartsWith(Project.AssetsPath)
                ? Project.AssetsPath.Length - "Assets".Length
                : path.IndexOf("Assets", StringComparison.InvariantCulture);

            if (assetsIndex < 0)
                throw new ArgumentException("Specified path does not refer to an assets path.", nameof(path));

            return path.Substring(assetsIndex);
        }

        #endregion Utilities
    }
}