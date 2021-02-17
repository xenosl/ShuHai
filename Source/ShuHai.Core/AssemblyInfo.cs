using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ShuHai.Core.Test")]

namespace ShuHai
{
    public static class AssemblyInfo
    {
        public const string Name = "ShuHai.Core";

        public const string RootNamespace = "ShuHai";

        public static readonly Assembly ThisAssembly = typeof(AssemblyInfo).Assembly;
    }
}
