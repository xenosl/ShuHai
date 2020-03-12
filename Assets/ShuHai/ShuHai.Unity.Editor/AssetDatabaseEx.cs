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
            var d = AssetDatabase.GetDependencies(assetPath, recursive);
            var e1 = includeSelf ? d : d.Except(assetPath.ToEnumerable());
            var e2 = ordered ? e1.OrderBy(s => s, StringComparer.InvariantCulture) : e1;
            return e2.ToArray();
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
            return EnumerateAllAssetPathsInFolder(folderPath).ToArray();
        }

        /// <summary>
        ///     Get a enumerable collection of asset paths in the folder of specified path.
        /// </summary>
        /// <param name="folderPath">Asset path of the folder to enumerate.</param>
        /// <param name="includeFolder">Determines whether the enumeration includes folder asset.</param>
        /// <returns>An enumerable collection of all asset paths in the folder of specified path.</returns>
        public static IEnumerable<string> EnumerateAllAssetPathsInFolder(string folderPath, bool includeFolder = false)
        {
            Ensure.Argument.NotNullOrEmpty(folderPath, nameof(folderPath));
            if (!AssetDatabase.IsValidFolder(folderPath))
                throw new ArgumentException("Folder asset path is required.", nameof(folderPath));

            var rootedFolderPath = UnityPath.ToRooted(folderPath);
            var assetPaths = includeFolder
                ? Directory.EnumerateFileSystemEntries(rootedFolderPath)
                : Directory.EnumerateFiles(rootedFolderPath);
            return assetPaths.Where(p => Path.GetExtension(p) != ".meta").Select(UnityPath.ToAsset);
        }
    }
}