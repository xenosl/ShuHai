using System;
using System.Collections.Generic;
using System.IO;
using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Assets.Editor
{
    public static class AssetManifestGenerator
    {
        #region File Generation

        public static void GenerateAssetFile(string outputDir, string filename)
        {
            var assets = AssetDatabaseEx.EnumerateAssetPaths();
            GenerateAssetFile(assets, outputDir, filename);
        }

        public static void GenerateAssetFile(IEnumerable<string> assetPaths, string outputDir, string filename)
        {
            GenerateFile(assetPaths, outputDir, filename, ".asset", AssetDatabase.CreateAsset);
        }

        public static void GenerateJsonFile(string outputDir, string filename)
        {
            var assets = AssetDatabaseEx.EnumerateAssetPaths();
            GenerateJsonFile(assets, outputDir, filename);
        }

        public static void GenerateJsonFile(IEnumerable<string> assetPaths, string outputDir, string filename)
        {
            GenerateFile(assetPaths, outputDir, filename, ".json", GenerateJsonFile);
        }

        private static void GenerateJsonFile(AssetManifest manifest, string path)
        {
            var json = JsonUtility.ToJson(manifest);
            path = UnityPath.ToRooted(path);
            File.WriteAllText(path, json);
        }

        private static void GenerateFile(IEnumerable<string> assetPaths,
            string outputDir, string filename, string ext, Action<AssetManifest, string> fileGenerator)
        {
            VerifyArguments(assetPaths, outputDir, filename);

            AssetDatabaseEx.CreateFolder(outputDir);
            var manifest = Generate(assetPaths);
            var path = PathEx.Combine(outputDir, filename + ext);
            fileGenerator(manifest, path);
            AssetDatabase.Refresh();
        }

        private static void VerifyArguments(IEnumerable<string> assetPaths, string outputDir, string filename)
        {
            Ensure.Argument.NotNull(assetPaths, nameof(assetPaths));
            Ensure.Argument.NotNullOrEmpty(outputDir, nameof(outputDir));
            Ensure.Argument.NotNullOrEmpty(filename, nameof(filename));
            if (!UnityPath.IsAssetPath(outputDir))
                throw new ArgumentException("Asset path expected.", nameof(outputDir));
        }

        #endregion File Generation

        #region Instance Generation

        public static AssetManifest Generate() { return Generate(AssetDatabaseEx.EnumerateAssetPaths()); }

        public static AssetManifest Generate(IEnumerable<string> assetPaths)
        {
            Ensure.Argument.NotNull(assetPaths, nameof(assetPaths));

            var manifest = AssetManifest.Create();

            try
            {
                foreach (var path in assetPaths)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (!importer)
                        continue;
                    
                    var bundleName = importer.assetBundleName;
                    if (!string.IsNullOrEmpty(bundleName))
                    {
                        var depends = AssetDatabaseEx.GetDependencies(path, true, true, p => CanAddDependency(path, p));
                        manifest.Add(path, bundleName, depends);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return manifest;
        }

        private static bool CanAddDependency(string assetPath, string dependencyPath)
        {
            if (dependencyPath == assetPath)
                return false;

            var ext = Path.GetExtension(dependencyPath);
            return ext != ".cs";
        }

        #endregion Instance Generation
    }
}