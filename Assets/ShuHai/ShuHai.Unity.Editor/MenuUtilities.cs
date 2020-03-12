using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ShuHai.Unity.Editor
{
    public static class MenuUtilities
    {
        #region Copy Assets

        private const string CopyMenuPath = "Assets/Copy";
        private const string PasteMenuPath = "Assets/Paste";

        [MenuItem(CopyMenuPath, true)]
        public static bool CanCopyAssets() { return !CollectionUtil.IsNullOrEmpty(Selection.objects); }

        [MenuItem(CopyMenuPath)]
        public static void CopyAssets()
        {
            _copyAssetsClipboard.Clear();

            var objects = Selection.objects;
            if (!CollectionUtil.IsNullOrEmpty(objects))
                _copyAssetsClipboard.AddRange(objects.Select(AssetDatabase.GetAssetPath));
        }

        [MenuItem(PasteMenuPath, true)]
        public static bool CanPasteAssets() { return _copyAssetsClipboard.Count > 0; }

        [MenuItem(PasteMenuPath)]
        public static void PasteAssets()
        {
            var folderPath = GetSelectedFolderPath();
            foreach (var path in _copyAssetsClipboard)
            {
                var newPath = GetPasteAssetPath(path, folderPath);
                AssetDatabase.CopyAsset(path, newPath);
            }
        }

        private static readonly List<string> _copyAssetsClipboard = new List<string>();

        private static string GetPasteAssetPath(string srcAssetPath, string folderPath)
        {
            var srcAssetName = Path.GetFileNameWithoutExtension(srcAssetPath);
            var srcAssetExt = Path.GetExtension(srcAssetPath);
            var assetNames = AssetDatabaseEx.EnumerateAllAssetPathsInFolder(folderPath, true)
                .Select(Path.GetFileNameWithoutExtension)
                .Where(p => string.IsNullOrEmpty(srcAssetExt) || !p.EndsWith(srcAssetExt));

            int index = 0;
            string assetName = srcAssetName;
            while (assetNames.Contains(assetName))
                assetName = $"{assetName} ({index++})";

            return $"{folderPath}/{assetName}{srcAssetExt}";
        }

        #endregion Copy Assets

        private static string GetSelectedFolderPath()
        {
            var obj = Selection.activeObject;
            if (!obj)
                return string.Empty;

            var assetPath = AssetDatabase.GetAssetPath(obj);
            return AssetDatabase.IsValidFolder(assetPath) ? assetPath : Path.GetDirectoryName(assetPath);
        }
    }
}