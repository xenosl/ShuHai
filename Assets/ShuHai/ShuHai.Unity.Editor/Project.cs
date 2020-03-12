using System;
using System.IO;
using UnityEngine;

namespace ShuHai.Unity.Editor
{
    public static class Project
    {
        /// <summary>
        ///     Name of current project, also the name of the folder where current project stores.
        /// </summary>
        public static string Name => _name.Value;

        private static readonly Lazy<string> _name = new Lazy<string>(() => PathEx.NameOf(RootPath));

        #region Paths

        /// <summary>
        ///     Root directory path of current project.
        /// </summary>
        public static string RootPath => _rootPath.Value;

        /// <summary>
        ///     The rooted path of "Assets" folder in project root directory.
        ///     See <see cref="Application.dataPath" /> for more information.
        /// </summary>
        public static string AssetsPath => Application.dataPath;

        /// <summary>
        ///     The rooted path of "Library" folder in project root directory.
        /// </summary>
        public static string LibraryPath => _libraryPath.Value;

        /// <summary>
        ///     The rooted path of "Library/ScriptAssemblies" folder in project root directory.
        /// </summary>
        public static string ScriptAssembliesPath => _scriptAssembliesPath.Value;

        /// <summary>
        ///     The rooted path of "ProjectSettings" folder in project root directory.
        /// </summary>
        public static string SettingsPath => _settingsPath.Value;

        /// <summary>
        ///     The rooted path of "Temp" folder in project root directory.
        /// </summary>
        public static string TempPath => _tempPath.Value;

        private static readonly Lazy<string> _rootPath = new Lazy<string>(() => Path.GetDirectoryName(AssetsPath));
        private static readonly Lazy<string> _libraryPath = new Lazy<string>(() => RootPath + "/Library");

        private static readonly Lazy<string> _scriptAssembliesPath
            = new Lazy<string>(() => LibraryPath + "/ScriptAssemblies");

        private static readonly Lazy<string> _settingsPath = new Lazy<string>(() => RootPath + "/ProjectSettings");
        private static readonly Lazy<string> _tempPath = new Lazy<string>(() => RootPath + "/Temp");

        #endregion Paths
    }
}