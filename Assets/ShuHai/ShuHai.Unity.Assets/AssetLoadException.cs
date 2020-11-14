using System;

namespace ShuHai.Unity.Assets
{
    public class AssetLoadException : Exception
    {
        public string AssetPath { get; }

        public override string Message
        {
            get
            {
                string msg = base.Message;
                return string.IsNullOrEmpty(AssetPath) ? msg : $"{msg}{Environment.NewLine}Asset path: {AssetPath}.";
            }
        }

        public AssetLoadException(string assetPath, string message = "Error occurs when loading asset.")
            : base(message)
        {
            AssetPath = assetPath;
        }

        public AssetLoadException(string message, Exception innerException) : base(message, innerException) { }
    }
}