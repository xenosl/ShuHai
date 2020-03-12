using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;

namespace ShuHai.Graphicode.Generator
{
    public static class UnitFileGenerator
    {
        public static CodeGeneratorOptions DefaultOptions
        {
            get => _defaultOptions;
            // Make sure the default options not null.
            set => _defaultOptions = (value ?? _defaultOptions) ?? new CodeGeneratorOptions();
        }

        private static CodeGeneratorOptions _defaultOptions = new CodeGeneratorOptions
        {
            BracingStyle = "C",
            VerbatimOrder = true
        };

        public static void GenerateCompileUnit(MethodInfo method, string dir)
        {
            GenerateCompileUnit(method, dir, DefaultOptions);
        }

        public static void GenerateCompileUnit(MethodInfo method, string dir, CodeGeneratorOptions options)
        {
            Ensure.Argument.NotNull(method, nameof(method));

            var classDom = UnitCodeGenerator.Create(method).GenerateClass();
            var compileUnit = new CodeCompileUnit
            {
                Namespaces =
                {
                    new CodeNamespace(UnitCodeGenerator.MakeNamespace(method))
                    {
                        Types = { classDom }
                    }
                }
            };

            string filename = MakeFilename(classDom.Name);
            string path = Path.Combine(dir, filename);
            using (var fileWriter = File.CreateText(path))
                CodeProvider.GenerateCodeFromCompileUnit(compileUnit, fileWriter, options);
        }

        public static string MakeFilename(string unitName) { return unitName + ".cs"; }

        /// <summary>
        ///     Generate unit class for all members of specified type into one compile unit.
        /// </summary>
        public static void GenerateCompileUnit(Type type)
        {
            if (!IsSupported(type))
                throw new NotSupportedException($@"Generate compile unit for ""{type.FullName}"" is not supported.");
        }

        public static bool IsSupported(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            return type.IsClass
                || type.IsValueType && !type.IsPrimitive;
        }

        private static readonly CodeDomProvider CodeProvider = CodeDomProvider.CreateProvider("CSharp");
    }
}