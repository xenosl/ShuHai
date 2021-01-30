using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ShuHai.Unity.Editor")]
[assembly: InternalsVisibleTo("ShuHai.Unity.Tests")]

namespace ShuHai.Unity
{
    public static class AssemblyInfo
    {
        public const string RootNamespace = ShuHai.AssemblyInfo.RootNamespace + ".Unity";

        public static readonly Assembly ThisAssembly = typeof(AssemblyInfo).Assembly;
    }
}
