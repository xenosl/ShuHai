using System;
using System.Collections.Generic;
using System.Text;

namespace ShuHai
{
    /// <summary>
    ///     Represents type name information needed to make up the type.
    /// </summary>
    public sealed class TypeName : IEquatable<TypeName>
    {
        public enum AssemblyNameFormat { Simple, Full }

        /// <summary>
        ///     Name of <see cref="System.Reflection.Assembly" /> which contains the type named by current instance, or
        ///     <see langword="null" /> if no <see cref="System.Reflection.Assembly" /> is specified to current instance.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        ///     The simple name of the type.
        ///     This is the name written in code without namespace for non-generic types, e.g. String, Int32, TypeName, etc; for
        ///     generic type this is the type definition name without its namespace.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Full name of the type, including its namespace but not its assembly.
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        ///     Namespace of the type.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        ///     Assembly-qualified name of the type, which includes <see cref="AssemblyName" /> if exists.
        /// </summary>
        public string AssemblyQualifiedName { get; }

        /// <summary>
        ///     The name used to find the type in single assembly - without generic arguments or array symbols.
        ///     e.g. Declare name of "System.Collections.Generic.Dictionary`2[System.String,System.Boolean][,]" is
        ///     "System.Collections.Generic.Dictionary`2".
        /// </summary>
        public string DeclareName { get; private set; }

        /// <summary>
        ///     Parent type name of current instance if current instance represents a generic argument type name of
        ///     certain generic type name.
        /// </summary>
        public TypeName GenericParent { get; }

        #region Construct

        private TypeName(string fullyQualifiedName)
        {
            Ensure.Argument.NotNullOrEmpty(fullyQualifiedName, nameof(fullyQualifiedName));

            AssemblyQualifiedName = TrimName(fullyQualifiedName);
            _hashCode = AssemblyQualifiedName.GetHashCode();

            ParseFullNameAndAssemblyNameFromAssemblyQualifiedName();

            ParseArrayRanksFromFullName(out int arrayBracketIndex);

            ParseDeclareNameAndGenericArgumentsFromFullName(arrayBracketIndex);

            ParseNamespaceAndNameFromDeclareName();
        }

        private TypeName(string fullyQualifiedName, TypeName genericParent)
            : this(fullyQualifiedName)
        {
            GenericParent = genericParent;
        }

        private void ParseFullNameAndAssemblyNameFromAssemblyQualifiedName()
        {
            int delimiterIndex = GetAssemblyDelimiterIndex(AssemblyQualifiedName);
            if (delimiterIndex == 0)
                throw new ArgumentException($"'{AssemblyQualifiedName}' is not a valid type name.");

            if (delimiterIndex > 0)
            {
                FullName = AssemblyQualifiedName.Substring(0, delimiterIndex).Trim();
                AssemblyName = AssemblyQualifiedName
                    .Substring(delimiterIndex + 1, AssemblyQualifiedName.Length - delimiterIndex - 1).Trim();
            }
            else
            {
                FullName = AssemblyQualifiedName;
                AssemblyName = null;
            }
        }

        private void ParseDeclareNameAndGenericArgumentsFromFullName(int arrayBracketIndex)
        {
            DeclareName = arrayBracketIndex >= 0 ? FullName.Substring(0, arrayBracketIndex) : FullName;

            if (FindFirstRootBracketPairIndices(DeclareName, out int leftBracketIndex, out int rightBracketIndex))
            {
                string argString = DeclareName.Substring(
                    leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1).Trim();
                DeclareName = DeclareName.Substring(0, leftBracketIndex).Trim();
                ParseGenericArguments(argString);
            }
        }

        private void ParseNamespaceAndNameFromDeclareName()
        {
            var nsArray = DeclareName.Split('.');
            int nsArrayLen = nsArray.Length;

            string nestedName = nsArray[nsArrayLen - 1];
            if (nsArrayLen > 1)
                Namespace = DeclareName.Substring(0, DeclareName.Length - nestedName.Length - 1);

            var nameArray = nestedName.Split('+');
            Name = nameArray[nameArray.Length - 1];
        }

        #endregion Construct

        #region Equality

        public static bool operator ==(TypeName l, TypeName r)
        {
            return EqualityComparer<TypeName>.Default.Equals(l, r);
        }

        public static bool operator !=(TypeName l, TypeName r) { return !(l == r); }

        public bool Equals(TypeName other)
        {
            return other != null && AssemblyQualifiedName == other.AssemblyQualifiedName;
        }

        public override bool Equals(object obj) { return Equals(obj as TypeName); }

        public override int GetHashCode() { return _hashCode; }

        private readonly int _hashCode;

        #endregion Equality

        #region To String

        public override string ToString() { return ToString(null); }

        public string ToString(AssemblyNameFormat? assemblyNameStyle) { return ToString(true, assemblyNameStyle); }

        /// <summary>
        ///     Build a string representing the current instance.
        /// </summary>
        /// <param name="withNamespace">Whether to include namespace name in the result string.</param>
        /// <param name="assemblyNameStyle">
        ///     Determines whether to append assembly name of the type, and in what format the assembly name is represented
        ///     if the value is not <see langword="null" />.
        /// </param>
        public string ToString(bool withNamespace, AssemblyNameFormat? assemblyNameStyle)
        {
            return ToString(new StringBuilder(), withNamespace, assemblyNameStyle).ToString();
        }

        public StringBuilder ToString(StringBuilder builder,
            bool withNamespace, AssemblyNameFormat? assemblyNameStyle)
        {
            Ensure.Argument.NotNull(builder, nameof(builder));

            builder.Append(withNamespace ? DeclareName : Name);
            AppendGenericArguments(builder, withNamespace, assemblyNameStyle);
            AppendArrayRanks(builder);
            AppendAssemblyName(builder, assemblyNameStyle);
            return builder;
        }

        private void AppendGenericArguments(StringBuilder builder,
            bool withNamespace, AssemblyNameFormat? assemblyNameStyle)
        {
            if (GenericArgumentCount == 0)
                return;

            builder.Append('[');

            foreach (var arg in GenericArguments)
            {
                bool appendAssemblyName = CanAppendAssemblyName(arg, assemblyNameStyle);
                if (appendAssemblyName)
                    builder.Append('[');
                arg.ToString(builder, withNamespace, assemblyNameStyle);
                if (appendAssemblyName)
                    builder.Append(']');
                builder.Append(',');
            }
            builder.RemoveTail(1); // Remove last ','

            builder.Append(']');
        }

        private void AppendArrayRanks(StringBuilder builder)
        {
            if (!IsArray)
                return;

            foreach (int rank in _arrayRanks)
            {
                builder.Append('[');
                for (var i = 1; i < rank; ++i)
                    builder.Append(',');
                builder.Append(']');
            }
        }

        private void AppendAssemblyName(StringBuilder builder, AssemblyNameFormat? assemblyNameStyle)
        {
            if (!CanAppendAssemblyName(this, assemblyNameStyle))
                return;

            builder.Append(',');
            builder.Append(' ');

            var style = assemblyNameStyle.Value;
            switch (style)
            {
                case AssemblyNameFormat.Simple:
                    foreach (char c in AssemblyName)
                    {
                        if (c == ',')
                            break;
                        builder.Append(c);
                    }
                    break;
                case AssemblyNameFormat.Full:
                    builder.Append(AssemblyName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assemblyNameStyle));
            }
        }

        private static bool CanAppendAssemblyName(TypeName typeName, AssemblyNameFormat? assemblyNameStyle)
        {
            return !string.IsNullOrEmpty(typeName.AssemblyName) && assemblyNameStyle.HasValue;
        }

        #endregion To String

        #region Array

        public bool IsArray => _arrayRanks.Count > 0;

        public int ArrayDeclareCount => _arrayRanks.Count;

        /// <summary>
        ///     Number of dimensions for each array declaration if current instance is a jagged array.
        /// </summary>
        public IEnumerable<int> ArrayRanks => _arrayRanks;

        /// <summary>
        ///     Get the number of dimensions array of current type instance.
        /// </summary>
        /// <param name="index">
        ///     Index to locate which array declaration to get. The value greater than 0 is only valid if current instance is a
        ///     jagged array.
        /// </param>
        public int GetArrayRank(int index = 0) { return _arrayRanks[index]; }

        private readonly List<int> _arrayRanks = new List<int>();

        private bool ParseArrayRanksFromFullName(out int bracketStartIndex)
        {
            bracketStartIndex = -1;

            int depth = 0, maxIndex = FullName.Length - 1, rank = 1;
            for (int i = maxIndex; i >= 0; --i)
            {
                var nonArrayChar = false;
                char c = FullName[i];
                switch (c)
                {
                    case ']':
                        depth++;
                        break;
                    case '[':
                        depth--;
                        if (depth == 0)
                        {
                            _arrayRanks.Add(rank);
                            rank = 1;
                        }
                        break;
                    case ',':
                        rank++;
                        break;
                    case ' ':
                    case '*':
                        break;
                    default: // Characters except "[*,] ".
                        nonArrayChar = true;
                        if (depth == 0 && i != maxIndex)
                            bracketStartIndex = i + 1;
                        break;
                }
                if (nonArrayChar)
                    break;
            }
            _arrayRanks.Reverse();

            return bracketStartIndex >= 0;
        }

        #endregion Array

        #region Generic Arguments

        public bool IsGeneric => GenericArgumentCount > 0;

        public int GenericArgumentCount => _genericArguments.Count;

        public IEnumerable<TypeName> GenericArguments => _genericArguments;

        public TypeName GetGenericArgument(int index) { return _genericArguments[index]; }

        private readonly List<TypeName> _genericArguments = new List<TypeName>();

        private void ParseGenericArguments(string argString)
        {
            var delimiterIndices = new List<int>(EnumRootDelimiterIndices(argString));
            if (delimiterIndices.Count > 0)
            {
                var startIndex = 0;
                foreach (int index in delimiterIndices)
                {
                    string nodeStr = argString.Substring(startIndex, index - startIndex);
                    AddGenericArgument(nodeStr);
                    startIndex = index + 1;
                }
                string lastNodeStr = argString.Substring(startIndex, argString.Length - startIndex);
                AddGenericArgument(lastNodeStr);
            }
            else
            {
                AddGenericArgument(argString);
            }
        }

        private void AddGenericArgument(string argString) { _genericArguments.Add(new TypeName(argString, this)); }

        #endregion Generic Arguments

        #region Instances

        public static TypeName Get(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            return Get(type.AssemblyQualifiedName);
        }

        public static TypeName Get(string fullyQualifiedName)
        {
            if (!instances.TryGetValue(fullyQualifiedName, out var name))
            {
                name = new TypeName(fullyQualifiedName);
                instances.Add(fullyQualifiedName, name);
            }
            return name;
        }

        private static readonly Dictionary<string, TypeName> instances = new Dictionary<string, TypeName>();

        #endregion Instances

        #region Utilities

        private static string TrimName(string name)
        {
            name = name.Trim();
            if (name.StartsWith("[") && name.EndsWith("]"))
                name = name.Substring(1, name.Length - 2);
            return name;
        }

        private static int GetAssemblyDelimiterIndex(string fullQualifiedName)
        {
            // We need to get the first comma following all surrounded in brackets because of generic types. e.g.
            // System.Collections.Generic.Dictionary`2[
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
            //   [System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]],
            //     mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            foreach (int index in EnumRootDelimiterIndices(fullQualifiedName))
                return index;
            return -1;
        }

        private static IEnumerable<int> EnumRootDelimiterIndices(string fullName)
        {
            var depth = 0;
            for (var i = 0; i < fullName.Length; i++)
            {
                char c = fullName[i];
                switch (c)
                {
                    case '[':
                        depth++;
                        break;
                    case ']':
                        depth--;
                        break;
                    case ',':
                        if (depth == 0)
                            yield return i;
                        break;
                }
            }
        }

        private static bool FindFirstRootBracketPairIndices(string fullName, out int leftIndex, out int rightIndex)
        {
            leftIndex = -1;
            rightIndex = -1;

            var depth = 0;
            for (var i = 0; i < fullName.Length; i++)
            {
                char c = fullName[i];
                switch (c)
                {
                    case '[':
                        if (depth == 0)
                            leftIndex = i;
                        depth++;
                        break;
                    case ']':
                        depth--;
                        if (depth == 0)
                            rightIndex = i;
                        break;
                }

                if (leftIndex >= 0 && rightIndex >= 0)
                    break;
            }

            return leftIndex >= 0 && rightIndex >= 0;
        }

        #endregion Utilities
    }
}
