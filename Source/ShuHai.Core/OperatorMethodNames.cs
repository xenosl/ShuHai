using System.Collections;
using System.Collections.Generic;

namespace ShuHai
{
    /// <summary>
    ///     Represents a collection that contains all names of overloadable operators.
    /// </summary>
    public sealed class OperatorMethodNames : IReadOnlyCollection<string>
    {
        public const string UnaryPlus = "op_UnaryPlus"; // +
        public const string UnaryNegation = "op_UnaryNegation"; // -
        public const string Increment = "op_Increment"; // ++
        public const string Decrement = "op_Decrement"; // --
        public const string LogicalNot = "op_LogicalNot"; // !
        public const string Addition = "op_Addition"; // + 
        public const string Subtraction = "op_Subtraction"; // -
        public const string Multiply = "op_Multiply"; // *
        public const string Division = "op_Division"; // /
        public const string BitwiseAnd = "op_BitwiseAnd"; // &
        public const string BitwiseOr = "op_BitwiseOr"; // |
        public const string ExclusiveOr = "op_ExclusiveOr"; // ^
        public const string Equality = "op_Equality"; // ==
        public const string Inequality = "op_Inequality"; // !=
        public const string LessThan = "op_LessThan"; // <
        public const string GreaterThan = "op_GreaterThan"; // >
        public const string LessThanOrEqual = "op_LessThanOrEqual"; // <=
        public const string GreaterThanOrEqual = "op_GreaterThanOrEqual"; // >=
        public const string LeftShift = "op_LeftShift"; // << 
        public const string RightShift = "op_RightShift"; // >>
        public const string Modulus = "op_Modulus"; // %
        public const string Implicit = "op_Implicit"; // Implicit type conversion
        public const string Explicit = "op_Explicit"; // Explicit type conversion
        public const string True = "op_True"; // true
        public const string False = "op_False"; // false

        public static readonly OperatorMethodNames Instance = new OperatorMethodNames();

        public int Count => _names.Count;

        public bool Contains(string name) { return _names.Contains(name); }

        public IEnumerator<string> GetEnumerator() { return _names.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        private readonly HashSet<string> _names;

        private OperatorMethodNames()
        {
            _names = new HashSet<string>
            {
                UnaryPlus, UnaryNegation, Increment, Decrement, Addition, Subtraction, Multiply, Division,
                LogicalNot, Modulus, BitwiseAnd, BitwiseOr, ExclusiveOr, LeftShift, RightShift,
                Equality, Inequality, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual,
                Implicit, Explicit, True, False
            };
        }
    }
}