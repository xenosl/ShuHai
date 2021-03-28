using System;
using System.Reflection;

namespace ShuHai.Unity
{
    public static class AssemblyCache
    {
        public const string PlayerAssemblyName = "Assembly-CSharp";
        public const string PlayerPluginsAssemblyName = "Assembly-CSharp-firstpass";

        public static Assembly UnityEngine => typeof(UnityEngine.Object).Assembly;

        public static Assembly Player => _player.Value;
        public static Assembly PlayerPlugins => _playerPlugins.Value;

        private static readonly Lazy<Assembly> _player =
            new Lazy<Assembly>(() => ShuHai.AssemblyCache.Get(PlayerAssemblyName));

        private static readonly Lazy<Assembly> _playerPlugins =
            new Lazy<Assembly>(() => ShuHai.AssemblyCache.Get(PlayerPluginsAssemblyName));

#if UNITY_EDITOR
        public const string EditorAssemblyName = "Assembly-CSharp-Editor";
        public const string EditorPluginsAssemblyName = "Assembly-CSharp-Editor-firstpass";

        public static Assembly UnityEditor => typeof(UnityEditor.Editor).Assembly;

        public static Assembly Editor => _editor.Value;
        public static Assembly EditorPlugins => _editor.Value;

        private static readonly Lazy<Assembly> _editor =
            new Lazy<Assembly>(() => ShuHai.AssemblyCache.Get(EditorAssemblyName));

        private static readonly Lazy<Assembly> _editorPlugins =
            new Lazy<Assembly>(() => ShuHai.AssemblyCache.Get(EditorPluginsAssemblyName));
#endif
    }
}
