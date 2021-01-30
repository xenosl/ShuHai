using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ShuHai.Unity.Editor
{
    /// <summary>
    ///     Extra methods of <see cref="AssetDatabase" />.
    /// </summary>
    public static class AssetDatabaseEx
    {
        /// <summary>
        ///     Equivalent to <see cref="AssetDatabase.GetDependencies(string, bool)" /> by default, but more options are
        ///     provided.
        /// </summary>
        /// <param name="assetPath">The path to the asset for which dependencies are required.</param>
        /// <param name="recursive">
        ///     If false, return only assets which are direct dependencies of the input; if true, include all indirect dependencies
        ///     of the input. Defaults to true.
        /// </param>
        /// <param name="includeSelf">Whether to include parameter <paramref name="assetPath" /> in result.</param>
        /// <param name="ordered">Order the result by descending if <see langword="true" />.</param>
        /// <returns>The paths of all assets that the input depends on.</returns>
        public static string[] GetDependencies(
            string assetPath, bool recursive = true, bool includeSelf = true, bool ordered = false)
        {
            var filter = includeSelf ? (Func<string, bool>)(p => true) : p => p != assetPath;
            return GetDependencies(assetPath, recursive, ordered, filter);
        }

        public static string[] GetDependencies(string assetPath,
            bool recursive, bool ordered, Func<string, bool> filter)
        {
            var d0 = AssetDatabase.GetDependencies(assetPath, recursive);
            var d1 = filter != null ? d0.Where(filter) : d0;
            var d2 = ordered ? d1.OrderBy(s => s, StringComparer.InvariantCulture) : d1;
            return d2.ToArray();
        }

        /// <summary>
        ///     Similar with <see cref="AssetDatabase.CreateFolder" />, but all missing intermediate folders in the specified
        ///     path are also created.
        /// </summary>
        /// <param name="path">Asset path of the folder to create.</param>
        /// <returns>
        ///     The GUIDs of all newly created folder; or an empty array if the specified path is already a valid folder.
        /// </returns>
        public static string[] CreateFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return Array.Empty<string>();

            var folders = PathEx.Split(path);
            if (!folders.Any())
                return Array.Empty<string>();

            var pathList = new List<string>();
            string current = string.Empty, parent = string.Empty;
            foreach (string name in folders)
            {
                current = parent != string.Empty ? parent + "/" + name : name;
                if (!AssetDatabase.IsValidFolder(current))
                    pathList.Add(AssetDatabase.CreateFolder(parent, name));
                parent = current;
            }

            return pathList.ToArray();
        }

        public static string[] GetAllAssetPathsInFolder(string folderPath)
        {
            return EnumerateAssetPathsInFolder(folderPath, false).ToArray();
        }

        /// <summary>
        ///     Get a enumerable collection of asset paths in the folder of specified path.
        /// </summary>
        /// <param name="folderPath">Asset path of the folder to enumerate.</param>
        /// <param name="includeFolder">Determines whether the enumeration includes folder asset.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in path. This parameter can contain a combination
        ///     of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current
        ///     directory or should include all subdirectories. The default value is <see cref="SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>An enumerable collection of all asset paths in the folder of specified path.</returns>
        public static IEnumerable<string> EnumerateAssetPathsInFolder(
            string folderPath, bool includeFolder = false, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Ensure.Argument.NotNullOrEmpty(folderPath, nameof(folderPath));
            if (!AssetDatabase.IsValidFolder(folderPath))
                throw new ArgumentException("Asset folder path is required.", nameof(folderPath));

            var rootedFolderPath = UnityPath.ToRooted(folderPath);
            var assetPaths = includeFolder
                ? Directory.EnumerateFileSystemEntries(rootedFolderPath, searchPattern, searchOption)
                : Directory.EnumerateFiles(rootedFolderPath, searchPattern, searchOption);
            return assetPaths.Where(p => Path.GetExtension(p) != ".meta").Select(UnityPath.ToAsset);
        }
        
        /// <summary>
        ///     Get a enumerable collection of asset paths in the folder of specified path.
        /// </summary>
        /// <param name="folderPath">Asset path of the folder to enumerate.</param>
        /// <param name="filter">Determines whether to add certain path to the result collection.</param>
        /// <param name="searchOption">
        ///     One of the enumeration values that specifies whether the search operation should include only the current
        ///     directory or should include all subdirectories. The default value is <see cref="SearchOption.TopDirectoryOnly" />.
        /// </param>
        /// <returns>An enumerable collection of all asset paths in the folder of specified path.</returns>
        public static IEnumerable<string> EnumerateAssetPathsInFolder(string folderPath,
            Func<string, bool> filter = null, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Ensure.Argument.NotNullOrEmpty(folderPath, nameof(folderPath));
            if (!AssetDatabase.IsValidFolder(folderPath))
                throw new ArgumentException("Asset folder path is required.", nameof(folderPath));

            var rootedFolderPath = UnityPath.ToRooted(folderPath);
            return Directory.EnumerateFileSystemEntries(rootedFolderPath, "*", searchOption)
                .Where(p => Path.GetExtension(p) != ".meta")
                .Select(UnityPath.ToAsset)
                .Where(p => filter == null || filter(p));
        }

        /// <summary>
        ///     Get a enumerable collection of all assets (filtered by the specified parameters) in current project.
        /// </summary>
        /// <param name="includeFolder">Determines whether the enumeration includes folder asset.</param>
        /// <param name="searchPattern">
        ///     The search string to match against the names of files in path. This parameter can contain a combination
        ///     of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateAssetPaths(bool includeFolder = false, string searchPattern = "*")
        {
            return EnumerateAssetPathsInFolder("Assets", includeFolder, searchPattern, SearchOption.AllDirectories);
        }
    }
}