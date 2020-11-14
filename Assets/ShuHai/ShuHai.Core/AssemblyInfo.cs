using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ShuHai.Core.Editor")]
[assembly: InternalsVisibleTo("ShuHai.Core.Tests")]

namespace ShuHai
{
    public static class AssemblyInfo
    {
        public const string RootNamespace = "ShuHai";

        public static readonly Assembly ThisAssembly = typeof(AssemblyInfo).Assembly;
    }
}
