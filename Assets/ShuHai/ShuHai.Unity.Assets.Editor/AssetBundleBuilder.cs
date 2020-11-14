using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ShuHai.Unity.Assets.Editor
{
    public class AssetBundleBuilder : IActiveBuildTargetChanged, IPreprocessBuildWithReport
    {
        public static void Build()
        {
            var dir = UnityPath.ToAsset(AssetBundles.RootDirectory);
            AssetDatabase.DeleteAsset(dir);
            AssetDatabaseEx.CreateFolder(dir);

            BuildPipeline.BuildAssetBundles(AssetBundles.RootDirectory,
                BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            AssetDatabase.Refresh();
        }

        #region Callbacks

        int IOrderedCallback.callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report) { Build(); }

        void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            Build();
        }

        #endregion Callbacks
    }
}