using System;
using System.Reflection;
using ShuHai.Graphicode.Generator;
using ShuHai.Unity.Editor;

namespace ShuHai.Graphicode.Unity.Editor
{
    public static class CodeGenerator
    {
        public static void Generate(MethodInfo method)
        {
            string dir = GetOutputDirectory(method);
            AssetDatabaseEx.CreateFolder(dir);
            UnitFileGenerator.GenerateCompileUnit(method, dir);
        }

        #region Path

        public const string RootDirectory = "Assets/ShuHai.Generated";

        public static string GetAssemblyDefinitionPath(Assembly assembly)
        {
            return $"{GetOutputDirectory(assembly)}/{GetAssemblyDefinitionName(assembly)}.asmdef";
        }

        public static string GetAssemblyDefinitionName(Assembly assembly)
        {
            var asmName = assembly.GetName();
            return $"{asmName.Name}-{asmName.Version}.Generated";
        }

        public static string GetOutputDirectory(MethodInfo method) { return GetOutputDirectory(method.DeclaringType); }

        public static string GetOutputDirectory(Type type)
        {
            return $"{GetOutputDirectory(type.Assembly)}/{type.Namespace}/{type.Name}";
        }

        public static string GetOutputDirectory(Assembly assembly)
        {
            return $"{RootDirectory}/{GetOutputDirectoryName(assembly)}";
        }

        private static string GetOutputDirectoryName(Assembly assembly)
        {
            var asmName = assembly.GetName();
            return $"{asmName.Name}-{asmName.Version}";
        }

        #endregion Path
    }
}
