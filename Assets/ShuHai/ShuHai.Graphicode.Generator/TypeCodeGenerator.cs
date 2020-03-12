using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai.Graphicode.Generator
{
    public class TypeCodeGenerator
    {
        public readonly Type Type;

        public TypeCodeGenerator(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            if (!type.IsClass && !type.IsValueType)
                throw new ArgumentException("Only class or struct is allowed.", nameof(type));
            if (type.IsGenericType && type.ContainsGenericParameters)
                throw new ArgumentException("Only closed constructed generic type is allowed.", nameof(type));

            Type = type;

            CollectMethods();
        }

        public CodeTypeDeclaration[] GenerateUnitClasses()
        {
            var results = from List<MethodInfo> overloads in _methods
                let option = overloads.Count > 1 ? _optionsForOverloads : UnitCodeGenerator.Options.Default
                from method in overloads
                select UnitCodeGenerator.Create(method).GenerateClass(option);
            return results.ToArray();
        }

        private static readonly UnitCodeGenerator.Options _optionsForOverloads = new UnitCodeGenerator.Options
        {
            AppendParameterTypeToName = true,
            AppendParameterTypeToClassName = true
        };

        private readonly Dictionary<string, List<MethodInfo>> _methods = new Dictionary<string, List<MethodInfo>>();

        private void CollectMethods()
        {
            foreach (var method in Type.GetMethods())
                _methods.Add(method.Name, method);
        }
    }
}
