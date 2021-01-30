using System;
using System.Reflection;
using System.Text;

namespace ShuHai
{
    public sealed class MethodNameOptions
    {
        #region Default

        public static MethodNameOptions Default
        {
            get => _default;
            set => _default = value ?? new MethodNameOptions(); // Make sure the default value always valid.
        }

        private static MethodNameOptions _default = new MethodNameOptions();

        #endregion Default

        public bool HasReturnType = true;
        public bool FullNameReturnType = false;

        public bool HasDeclaringType = false;
        public bool FullNameDeclaringType = false;

        public bool HasGenericArguments = true;
        public bool FullNameGenericArguments = false;

        public bool HasParameterTypes = true;
        public bool FullNameParameterTypes = false;

        public bool HasParameterNames = true;
    }

    public static class MethodInfoExtension
    {
        public static string MakeName(this MethodInfo self) { return MakeName(self, MethodNameOptions.Default); }

        public static string MakeName(this MethodInfo self, MethodNameOptions options)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(options, nameof(options));

            var builder = new StringBuilder();

            var separator = string.Empty;
            var appendSeparator = new Action(() =>
            {
                if (!string.IsNullOrEmpty(separator))
                    builder.Append(separator);
                separator = string.Empty;
            });

            if (options.HasReturnType)
            {
                var retType = self.ReturnType;
                builder.Append(options.FullNameReturnType ? retType.FullName : retType.Name);
                separator = " ";
            }

            if (options.HasDeclaringType)
            {
                appendSeparator();
                var declType = self.DeclaringType;
                builder.Append(options.FullNameDeclaringType ? declType.FullName : declType.Name);
                separator = ".";
            }

            appendSeparator();
            builder.Append(self.Name);

            if (options.HasGenericArguments && self.IsGenericMethod)
            {
                builder.Append("<");
                var genArgs = self.GetGenericArguments();
                foreach (var type in genArgs)
                {
                    builder.Append(options.FullNameGenericArguments ? type.FullName : type.Name);
                    builder.Append(", ");
                }
                builder.RemoveTail(2);
                builder.Append(">");
            }

            if (options.HasParameterTypes || options.HasParameterNames)
            {
                builder.Append("(");
                var parameters = self.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (options.HasParameterTypes)
                    {
                        var type = parameter.ParameterType;
                        builder.Append(options.FullNameParameterTypes ? type.FullName : type.Name);
                    }
                    if (options.HasParameterNames)
                    {
                        builder.Append(" ").Append(parameter.Name);
                    }
                    builder.Append(", ");
                }
                if (parameters.Length > 0)
                    builder.RemoveTail(2);
                builder.Append(")");
            }

            return builder.ToString();
        }

        public static bool IsOverloadableOperator(this MethodInfo self)
        {
            return self.IsSpecialName && OperatorMethodNames.Instance.Contains(self.Name);
        }
    }
}
