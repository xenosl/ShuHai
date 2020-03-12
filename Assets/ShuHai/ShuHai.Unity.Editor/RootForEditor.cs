using UnityEditor;

namespace ShuHai.Unity.Editor
{
    internal static class RootForEditor
    {
        private static void Update() { Root.EditorRootUpdate(); }

        [InitializeOnLoadMethod]
        private static void Initialize() { EditorApplication.update += Update; }
    }
}