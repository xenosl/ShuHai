using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Assets.Editor
{
    public static class MenuItems
    {
        public const string Path = MenuInfo.Path + "Assets/";

        [MenuItem(Path + "Build Bundles")]
        public static void BuildBundles() { AssetBundleBuilder.Build(); }

        [MenuItem(Path + "Generate Manifest")]
        public static void GenerateManifest()
        {
            var dir = UnityPath.ToAsset(Application.streamingAssetsPath);
            AssetManifestGenerator.GenerateJsonFile(dir, "Manifest");
        }

        [MenuItem(Path + "Build All")]
        public static void BuildAll()
        {
            BuildBundles();
            GenerateManifest();
        }
    }
}