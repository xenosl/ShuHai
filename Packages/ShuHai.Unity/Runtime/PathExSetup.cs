namespace ShuHai.Unity
{
    internal static class PathExSetup
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif // UNITY_EDITOR
        private static void Initialize() { PathEx.DirectorySeparatorStyle = DirectorySeparatorStyle.Slash; }
    }
}
