using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ShuHai.Graphicode.Generator
{
    public sealed class UnitCodeGenerator
    {
        public class Options
        {
            public static Options Default
            {
                get => _default;
                set => _default = value ?? _default; // Ensure the property always not null.
            }

            public static Options ForOverloads
            {
                get => _forOverloads;
                set => _forOverloads = value ?? _forOverloads; // Ensure the property always not null.
            }

            /// <summary>
            ///     Determines whether name of the generated unit class appends with names of parameter types. This is useful
            ///     when overload methods exists.
            /// </summary>
            public bool AppendParameterTypeToClassName;

            /// <summary>
            ///     Determines whether to append names of parameter types to default name (of the <see cref="Unit.Name" />
            ///     property) of the generated unit class.
            /// </summary>
            public bool AppendParameterTypeToName;

            private static Options _default = new Options();

            private static Options _forOverloads = new Options
            {
                AppendParameterTypeToName = true,
                AppendParameterTypeToClassName = true
            };
        }

        public static UnitCodeGenerator Create(MethodInfo method)
        {
            Ensure.Argument.NotNull(method, nameof(method));

            if (!IsSupported(method))
            {
                throw new NotSupportedException(
                    $@"Generating Unit class for method ""{method.Name}"" is not supported.");
            }

            return new UnitCodeGenerator(method);
        }

        public static bool IsSupported(MethodInfo method)
        {
            if (method.IsSpecialName) // Property and operator methods
                return false;

            if (method.IsDefined(typeof(ObsoleteAttribute)))
                return false;

            var parameters = method.GetParameters();
            if (parameters.Any(p => p.ParameterType.IsByRef && !p.IsOut)) // Parameter has "ref" prefix.
                return false;

            return true;
        }

        /// <summary>
        ///     From which method the current generator generates the unit class.
        /// </summary>
        public readonly MethodInfo Method;

        public CodeTypeDeclaration GenerateClass()
        {
            var typeCache = TypeCache.Get(Method.DeclaringType);
            bool hasOverload = typeCache.Methods[Method.Name].Count > 1;
            var options = hasOverload ? Options.ForOverloads : Options.Default;
            return GenerateClass(options);
        }

        /// <summary>
        ///     Generate code of unit class for the <see cref="Method" />.
        /// </summary>
        public CodeTypeDeclaration GenerateClass(Options options)
        {
            var classDecl = new CodeTypeDeclaration
            {
                // public class [MethodName]Unit : Unit
                TypeAttributes = TypeAttributes.Public,
                IsClass = true,
                Name = MakeClassName(Method, options.AppendParameterTypeToClassName),
                BaseTypes = { UnitInfo.Type },
                //CustomAttributes = { GenerateAttribute<GeneratedCodeAttribute>() },
            };

            var members = classDecl.Members;
            members.Add(NewDefaultConstructor(options.AppendParameterTypeToName));
            members.Add(NewStringConstructor());
            members.Add(NewConstructMemberMethod());
            members.AddRange(NewInputPortProperties());
            members.AddRange(NewOutputPortProperties());
            members.Add(NewExecuteImplMemberMethod());

            return classDecl;
        }

        private UnitCodeGenerator(MethodInfo method)
        {
            if (method.IsGenericMethod && method.ContainsGenericParameters)
                throw new ArgumentException("Only closed constructed generic method is allowed.", nameof(method));

            Method = method;
            _parameters = method.GetParameters();

            _inputValuePorts = CreateInputValuePorts();
            _outputValuePorts = CreateOutputValuePorts();
            _invokeParameters = CreateInvokeParameters();
        }

        #region Intermediate Data

        private bool HasReturnValue => Method.ReturnType != typeof(void);

        private readonly IReadOnlyList<ParameterInfo> _parameters;

        /// <summary>
        ///     Class (partial) layout of the generated unit type.
        /// </summary>
        private static class Layout
        {
            public static class ConstructMethod
            {
                public const string Name = "Construct";
            }
        }

        private static class UnitInfo
        {
            public static readonly Type Type = typeof(Unit);

            public static readonly PropertyInfo NameProperty = Type.BaseType.FindProperty("Name");

            public const string InputValuePortsPropertyName = "InputValuePorts";
            public const string OutputValuePortsPropertyName = "OutputValuePorts";

            public const string AddInputValuePortMethodName = "AddInputValuePort";
            public const string AddOutputValuePortMethodName = "AddOutputValuePort";

            public static readonly MethodInfo ExecuteImplMethod = Type.FindMethod("ExecuteImpl");
        }

        private static CodeMethodReferenceExpression NewAddInputValuePortMethodReference(Type valueType)
        {
            return new CodeMethodReferenceExpression
            {
                TargetObject = new CodeThisReferenceExpression(),
                MethodName = MakeClosedConstructedMethodName(UnitInfo.AddInputValuePortMethodName, valueType.FullName)
            };
        }

        private static CodeMethodReferenceExpression NewAddOutputValuePortMethodReference(Type valueType)
        {
            return new CodeMethodReferenceExpression
            {
                TargetObject = new CodeThisReferenceExpression(),
                MethodName = MakeClosedConstructedMethodName(UnitInfo.AddOutputValuePortMethodName, valueType.FullName)
            };
        }

        private static string MakeClosedConstructedMethodName(string methodName, string typeName)
        {
            return methodName + "<" + typeName + ">";
        }

        private static class PortInfo
        {
            public const string ValuePropertyName = "Value";
        }

        #region Ports

        /// <summary>
        ///     From where the port value comes from.
        /// </summary>
        private enum ValueSource
        {
            ThisReference,
            Parameter,
            Return
        }

        private sealed class ValuePortInfo
        {
            public static ValuePortInfo FromThisReference(int portIndex, Type thisType)
            {
                return new ValuePortInfo
                {
                    Name = thisType.Name,
                    PropertyName = thisType.Name + "Port",
                    IsOutput = false,
                    ValueType = thisType,
                    PortIndex = portIndex,
                    ValueSource = ValueSource.ThisReference,
                    Parameter = null,
                    ParameterIndex = Index.Invalid
                };
            }

            public static ValuePortInfo FromParameter(int portIndex, ParameterInfo parameter, int parameterIndex)
            {
                bool isOutput = IsOutputParameter(parameter);
                var rawParamType = GetRawType(parameter);
                string name = parameter.IsRetval ? "Returns" : parameter.Name;
                var info = new ValuePortInfo
                {
                    Name = name,
                    PropertyName = name + "Port",
                    IsOutput = isOutput,
                    ValueType = rawParamType,
                    PortIndex = portIndex,
                    ValueSource = parameter.IsRetval ? ValueSource.Return : ValueSource.Parameter,
                    Parameter = parameter,
                    ParameterIndex = parameterIndex
                };
                return info;
            }

            /// <summary>
            ///     Display name of current port.
            /// </summary>
            public string Name { get; private set; }

            public string PropertyName { get; private set; }

            public bool IsOutput { get; private set; }

            public Type ValueType { get; private set; }

            public int PortIndex { get; private set; }

            public ValueSource ValueSource { get; private set; }

            public ParameterInfo Parameter { get; private set; }

            /// <summary>
            ///     <see cref="Parameter" /> index in method invocation parameter list if the <see cref="Parameter" />
            ///     is one of the invoke parameters (which gets by <see cref="MethodInfo.GetParameters()" />); otherwise,
            ///     value of this property is -1.
            /// </summary>
            public int ParameterIndex { get; private set; }
        }

        private readonly IReadOnlyList<ValuePortInfo> _inputValuePorts;
        private readonly IReadOnlyList<ValuePortInfo> _outputValuePorts;

        private IEnumerable<ValuePortInfo> OutputPortsExceptRetVal
            => _outputValuePorts.Where(port => !port.Parameter.IsRetval);

        private List<ValuePortInfo> CreateInputValuePorts()
        {
            var list = new List<ValuePortInfo>();

            if (!Method.IsStatic)
                list.Add(ValuePortInfo.FromThisReference(list.Count, Method.DeclaringType));

            for (var i = 0; i < _parameters.Count; ++i)
            {
                var param = _parameters[i];
                if (!IsOutputParameter(param))
                    list.Add(ValuePortInfo.FromParameter(list.Count, param, i));
            }

            return list;
        }

        private List<ValuePortInfo> CreateOutputValuePorts()
        {
            var list = new List<ValuePortInfo>();

            if (HasReturnValue)
                list.Add(ValuePortInfo.FromParameter(list.Count, Method.ReturnParameter, -1));

            for (var i = 0; i < _parameters.Count; ++i)
            {
                var param = _parameters[i];
                if (IsOutputParameter(param))
                    list.Add(ValuePortInfo.FromParameter(list.Count, param, i));
            }

            return list;
        }

        private static int IndexOf(IReadOnlyList<ValuePortInfo> ports, ParameterInfo param)
        {
            return ports.IndexOf(p => p.Parameter == param);
        }

        private static CodePropertyReferenceExpression NewPortValueReferenceByPropertyExpression(ValuePortInfo port)
        {
            return new CodePropertyReferenceExpression
            {
                TargetObject = NewPortReferenceByPropertyExpression(port),
                PropertyName = PortInfo.ValuePropertyName
            };
        }

        private static CodePropertyReferenceExpression NewPortReferenceByPropertyExpression(ValuePortInfo port)
        {
            return new CodePropertyReferenceExpression
            {
                TargetObject = new CodeThisReferenceExpression(),
                PropertyName = port.PropertyName
            };
        }

        private static CodePropertyReferenceExpression NewPortValueReferenceByIndexExpression(ValuePortInfo port)
        {
            return new CodePropertyReferenceExpression
            {
                TargetObject = NewPortReferenceByIndexExpression(port),
                PropertyName = PortInfo.ValuePropertyName
            };
        }

        private static CodeCastExpression NewPortReferenceByIndexExpression(ValuePortInfo port)
        {
            string propertyName = port.IsOutput
                ? UnitInfo.OutputValuePortsPropertyName
                : UnitInfo.InputValuePortsPropertyName;

            return new CodeCastExpression
            {
                TargetType = NewPortTypeReference(port),
                Expression = new CodeIndexerExpression
                {
                    TargetObject = new CodePropertyReferenceExpression
                    {
                        TargetObject = new CodeThisReferenceExpression(),
                        PropertyName = propertyName
                    },
                    Indices = { new CodePrimitiveExpression(port.PortIndex) }
                }
            };
        }

        private static CodeTypeReference NewPortTypeReference(ValuePortInfo port)
        {
            var t = port.IsOutput ? typeof(IOutputValuePort<>) : typeof(IInputValuePort<>);
            return new CodeTypeReference(t.MakeGenericType(port.ValueType));
        }

        #endregion Ports

        #region Invoke Parameters

        private sealed class InvokeParameterInfo
        {
            public readonly ParameterInfo Parameter;
            public readonly Type RawParameterType;

            public readonly bool IsOutput;
            public readonly int PortIndex;

            public InvokeParameterInfo(ParameterInfo parameter, int portIndex)
            {
                Parameter = parameter;
                RawParameterType = GetRawType(parameter);
                IsOutput = IsOutputParameter(parameter);
                PortIndex = portIndex;
            }
        }

        private readonly IReadOnlyList<InvokeParameterInfo> _invokeParameters;

        private List<InvokeParameterInfo> CreateInvokeParameters()
        {
            return _parameters.Select(CreateInvokeParameterInfo).ToList();
        }

        private InvokeParameterInfo CreateInvokeParameterInfo(ParameterInfo param, int index)
        {
            int portIndex = param.IsOut ? IndexOf(_outputValuePorts, param) : IndexOf(_inputValuePorts, param);
            return new InvokeParameterInfo(param, portIndex);
        }

        private static bool IsOutputParameter(ParameterInfo param) { return param.IsRetval || param.IsOut; }

        #endregion Invoke Parameters

        #endregion Intermediate Data

        #region Generate Constructors

        private CodeConstructor NewDefaultConstructor(bool appendParameterTypesToName)
        {
            // public [MethodName]Unit() : base("[Method.Name]") { Construct(); }
            var dom = new CodeConstructor
            {
                Attributes = MemberAttributes.Public,
                BaseConstructorArgs =
                    { new CodePrimitiveExpression(MakeDefaultNamePropertyValue(Method, appendParameterTypesToName)) },
                Statements = { NewConstructMethodInvokeExpression() }
            };
            return dom;
        }

        private static CodeConstructor NewStringConstructor()
        {
            // public [MethodName]Unit(string name) : base(name) { Construct(); }
            const string arg0Name = "name";
            var dom = new CodeConstructor
            {
                Attributes = MemberAttributes.Public,
                Parameters = { new CodeParameterDeclarationExpression(typeof(string), arg0Name) },
                BaseConstructorArgs = { new CodeVariableReferenceExpression(arg0Name) },
                Statements = { NewConstructMethodInvokeExpression() }
            };
            return dom;
        }

        private static CodeMethodInvokeExpression NewConstructMethodInvokeExpression()
        {
            return new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), Layout.ConstructMethod.Name);
        }

        private CodeMemberMethod NewConstructMemberMethod()
        {
            // private void Construct() { /* Define ports base on method input/output. */ }
            var dom = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Private,
                Name = Layout.ConstructMethod.Name,
                ReturnType = new CodeTypeReference(typeof(void)),
            };
            dom.Statements.AddRange(NewAddNewPortInvokeStatements(_inputValuePorts));
            dom.Statements.AddRange(NewAddNewPortInvokeStatements(_outputValuePorts));
            return dom;
        }

        private static CodeExpressionStatement[] NewAddNewPortInvokeStatements(IEnumerable<ValuePortInfo> parameters)
        {
            return parameters
                .Select(NewAddNewPortInvokeStatement)
                .Select(e => new CodeExpressionStatement(e))
                .ToArray();
        }

        private static CodeMethodInvokeExpression NewAddNewPortInvokeStatement(ValuePortInfo port)
        {
            var methodDom = port.IsOutput
                ? NewAddOutputValuePortMethodReference(port.ValueType)
                : NewAddInputValuePortMethodReference(port.ValueType);

            return new CodeMethodInvokeExpression
            {
                Method = methodDom,
                Parameters = { new CodePrimitiveExpression(port.Name) }
            };
        }

        #endregion Generate Constructors

        #region Generate Port Properties

        private CodeMemberProperty[] NewInputPortProperties() { return NewPortProperties(_inputValuePorts).ToArray(); }

        private CodeMemberProperty[] NewOutputPortProperties()
        {
            return NewPortProperties(_outputValuePorts).ToArray();
        }

        private static IEnumerable<CodeMemberProperty> NewPortProperties(IEnumerable<ValuePortInfo> ports)
        {
            return ports.Select(NewPortProperty);
        }

        private static CodeMemberProperty NewPortProperty(ValuePortInfo port)
        {
            // A possible result:
            // public IOutputValuePort<string> resultPort { return (IOutputValuePort<string>)OutputValuePorts[1]; }

            return new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Type = NewPortTypeReference(port),
                Name = port.PropertyName,
                GetStatements = { new CodeMethodReturnStatement(NewPortReferenceByIndexExpression(port)) }
            };
        }

        #endregion Generate Port Properties

        #region Generate Execute Method

        private CodeMemberMethod NewExecuteImplMemberMethod()
        {
            // protected override void ExecuteImpl() { /* Invoke the source method and set results to output ports. */ }
            var dom = new CodeMemberMethod
            {
                Attributes = MemberAttributes.Family | MemberAttributes.Override,
                Name = UnitInfo.ExecuteImplMethod.Name,
                ReturnType = new CodeTypeReference(typeof(void))
            };
            dom.Statements.AddRange(NewMethodInvokeStatements());
            return dom;
        }

        private CodeStatementCollection NewMethodInvokeStatements()
        {
            var dom = new CodeStatementCollection();

            // Declare output parameters for the invocation.
            foreach (var port in OutputPortsExceptRetVal)
            {
                var varDeclDom = new CodeVariableDeclarationStatement(
                    new CodeTypeReference(GetRawType(port.Parameter)), MakeOutVariableName(port));
                dom.Add(varDeclDom);
            }

            // Invoke the method.
            var invokeParamDoms = new List<CodeExpression>();
            foreach (var param in _invokeParameters)
            {
                if (param.IsOutput)
                {
                    var port = _outputValuePorts[param.PortIndex];
                    invokeParamDoms.Add(new CodeSnippetExpression("out " + MakeOutVariableName(port)));
                }
                else
                {
                    var port = _inputValuePorts[param.PortIndex];
                    invokeParamDoms.Add(NewPortValueReferenceByPropertyExpression(port));
                }
            }

            var methodInvokeDom = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    TargetObject = Method.IsStatic
                        ? null
                        : NewPortValueReferenceByPropertyExpression(_inputValuePorts[0]),
                    MethodName = MakeMethodInvokeName(true)
                }
            };
            methodInvokeDom.Parameters.AddRange(invokeParamDoms.ToArray());

            var invokeDom = HasReturnValue
                ? new CodeAssignStatement(NewPortValueReferenceByPropertyExpression(_outputValuePorts[0]),
                    methodInvokeDom)
                : new CodeExpressionStatement(methodInvokeDom) as CodeStatement;
            dom.Add(invokeDom);

            // Assign output values.
            foreach (var port in OutputPortsExceptRetVal)
            {
                var setValueDom = new CodeAssignStatement(
                    NewPortValueReferenceByPropertyExpression(port),
                    new CodeVariableReferenceExpression(MakeOutVariableName(port)));
                dom.Add(setValueDom);
            }

            return dom;
        }

        private string MakeMethodInvokeName(bool containNamespace)
        {
            if (!Method.IsStatic)
                return Method.Name;

            var declType = Method.DeclaringType;
            Debug.Assert(declType != null, nameof(declType) + " != null");
            string typeName = containNamespace ? declType.FullName : declType.Name;
            return typeName + "." + Method.Name;
        }

        private string MakeOutVariableName(ValuePortInfo port)
        {
            return MakeOutVariableName(port.Parameter.IsRetval, port.PortIndex);
        }

        private string MakeOutVariableName(bool isRetval, int portIndex)
        {
            return isRetval ? "ret" : "out" + portIndex;
        }

        #endregion Generate Execute Method

        #region Utilities

        public string MakeDefaultNamePropertyValue(MethodInfo method, bool appendParameterTypes)
        {
            var b = new StringBuilder(method.Name);

            if (appendParameterTypes)
            {
                b.Append('(');
                foreach (var portInfo in _inputValuePorts)
                    b.Append(portInfo.Parameter.ParameterType.Name).Append(',');
                b.RemoveTail(1);
                b.Append(')');
            }

            return b.ToString();
        }

        public string MakeClassName(MethodInfo method, bool appendParameterTypes)
        {
            var b = new StringBuilder(method.Name);
            b.Append(UnitInfo.Type.Name);

            if (appendParameterTypes)
            {
                foreach (var portInfo in _inputValuePorts)
                    b.Append('_').Append(portInfo.Parameter.ParameterType.Name);
            }

            return b.ToString();
        }

        public static string MakeNamespace(Type type) { return $"{type.Namespace}.Generated.{type.Name}"; }

        public static string MakeNamespace(MethodInfo method)
        {
            Ensure.Argument.NotNull(method, nameof(method));
            return MakeNamespace(method.DeclaringType);
        }

        private static Type GetRawType(ParameterInfo param)
        {
            var type = param.ParameterType;
            return type.IsByRef ? type.GetElementType() : type;
        }

        #endregion Utilities
    }
}
