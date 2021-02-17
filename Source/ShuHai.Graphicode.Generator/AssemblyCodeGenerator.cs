using System;
using System.CodeDom;
using System.Reflection;

namespace ShuHai.Graphicode.Generator
{
    public sealed class AssemblyCodeGenerator
    {
        public readonly Assembly Assembly;

        public AssemblyCodeGenerator(Assembly assembly) { Assembly = assembly; }

        public CodeTypeDeclaration[] GenerateUnitClasses()
        {
            throw new NotImplementedException();
        }
    }
}