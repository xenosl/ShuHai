using System;
using NUnit.Framework;
using UnityEngine;

namespace ShuHai.Unity.Editor
{
    public class UnityPathTests
    {
        [Test]
        public void Constructor()
        {
            {
                var p = new UnityPath(AsRootedPath(@"MyProject\Assets\Folder1/test.asset"));
                Assert.AreEqual(true, p.IsValid);
                Assert.AreEqual("Assets/Folder1/test.asset", p.Asset);
                Assert.AreEqual(AsRootedPath("MyProject/Assets/Folder1/test.asset"), p.Rooted);
                Assert.AreEqual(".asset", p.Extension);
            }
            {
                var p = new UnityPath(@"Assets\Folder1/test.asset");
                Assert.AreEqual(true, p.IsValid);
                Assert.AreEqual("Assets/Folder1/test.asset", p.Asset);
                Assert.AreEqual(Application.dataPath + "/Folder1/test.asset", p.Rooted);
                Assert.AreEqual(".asset", p.Extension);
            }
            {
                var p = new UnityPath(@"MyProject\Assets\Folder1/test.png");
                Assert.AreEqual(false, p.IsValid);
                Assert.AreEqual(string.Empty, p.Asset);
                Assert.AreEqual(string.Empty, p.Rooted);
                Assert.AreEqual(".png", p.Extension);
            }
        }

        [Test]
        public void ToRooted()
        {
            var r = Application.dataPath;
            Assert.AreEqual(r + "/test.asset", UnityPath.ToRooted("Assets/test.asset"));
            Assert.AreEqual(r + "/Folder/test.asset", UnityPath.ToRooted(@"Assets\\\Folder\test.asset"));
        }

        [Test]
        public void ToAsset()
        {
            Assert.AreEqual("Assets/Folder1/test.asset",
                UnityPath.ToAsset(AsRootedPath(@"MyProject\Assets\Folder1/test.asset")));
            Assert.AreEqual("Assets/test.asset", UnityPath.ToAsset(Application.dataPath + "/test.asset"));
        }

        public static string AsRootedPath(string path, string windowsVolume = "D")
        {
            return GetRootedPathPrefix(windowsVolume) + path;
        }

        public static string GetRootedPathPrefix(string windowsVolume = "D")
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return windowsVolume + ":/";
                case PlatformID.Xbox:
                    throw new NotImplementedException();
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    return "/";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}