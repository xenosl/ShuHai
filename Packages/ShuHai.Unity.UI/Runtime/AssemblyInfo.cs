using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ShuHai.Unity.UI.Editor")]

namespace ShuHai.Unity.UI
{
    public static class AssemblyInfo
    {
        public const string RootNamespace = ShuHai.Unity.AssemblyInfo.RootNamespace + ".UI";

        public static readonly Assembly ThisAssembly = typeof(AssemblyInfo).Assembly;
    }
}
