using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai
{
    public static class Ensure
    {
        public static class Argument
        {
            public static void NotNull<T>(T arg, string name)
            {
                if (ReferenceEquals(arg, null))
                    throw new ArgumentNullException(name);
            }

            public static void NotNull<T>(T arg, string name, string message)
            {
                if (ReferenceEquals(arg, null))
                    throw new ArgumentNullException(name, message);
            }

            public static void AllNotNull<T>(IEnumerable<T> arg, string name)
            {
                if (arg is IReadOnlyList<T> list)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (list[i] == null)
                            throw new ArgumentNullException($"{name}[{i}]");
                    }
                }
                else
                {
                    int i = 0;
                    foreach (var item in arg)
                    {
                        if (item == null)
                            throw new ArgumentNullException($"{name}[{i}]");
                        i++;
                    }
                }
            }

            public static void NotEmpty<T>(IEnumerable<T> arg, string name)
            {
                if (!arg.Any())
                    throw new ArgumentException("Argument is empty.", name);
            }

            public static void NotEmpty<T>(IEnumerable<T> arg, string name, string message)
            {
                if (!arg.Any())
                    throw new ArgumentException(message, name);
            }

            public static void NotEmpty(string arg, string name)
            {
                if (arg.Length == 0)
                    throw new ArgumentException("Argument is empty.", name);
            }

            public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string name)
            {
                NotNull(arg, name);
                NotEmpty(arg, name);
            }

            public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string name, string message)
            {
                NotNull(arg, name, message);
                NotEmpty(arg, name, message);
            }

            public static void NotNullOrEmpty(string arg, string name)
            {
                NotNull(arg, name);
                NotEmpty(arg, name);
            }

//            public static void InRange<T>(T arg, T min, T max, string name)
//                where T : IComparable<T>
//            {
//                InRange(arg, min, max, true, name, null);
//            }
//
//            public static void InRange<T>(T arg, T min, T max, string name, string message)
//                where T : IComparable<T>
//            {
//                InRange(arg, min, max, true, name, message);
//            }
//
//            public static void InRange<T>(T arg, T min, T max, bool closed, string name, string message)
//                where T : IComparable<T>
//            {
//                if (!Interval<T>.Contains(min, max, arg, closed))
//                    throw new ArgumentOutOfRangeException(name, message);
//            }

            /// <summary>
            ///     Throws <see cref="ArgumentOutOfRangeException" /> if the specified argument take as index is not a valid
            ///     index within the specified range.
            /// </summary>
            /// <param name="arg">The argument to test.</param>
            /// <param name="length">Length of the range for testing.</param>
            /// <param name="name">Name of the argument.</param>
            public static void ValidIndex(int arg, int length, string name)
            {
                if (arg < 0 || arg > length - 1)
                    throw new ArgumentOutOfRangeException(name);
            }

            /// <summary>
            ///     Throws <see cref="ArgumentOutOfRangeException"/> if the specified argument take as index is not a valid
            ///     index with the range of the specified list.
            /// </summary>
            /// <param name="arg">The index to test.</param>
            /// <param name="list">The list from which to take the range.</param>
            /// <param name="name">Name of the argument.</param>
            public static void ValidIndex<T>(int arg, IReadOnlyList<T> list, string name)
            {
                ValidIndex(arg, list.Count, name);
            }

            #region Type Constraints

            public static void Is<TConstraint>(Type arg, string name) { Is(arg, name, typeof(TConstraint)); }

            /// <summary>
            ///     Ensure <paramref Name="arg" /> is sub type of <paramref Name="constraint" /> or same as
            ///     <paramref Name="constraint" />.
            /// </summary>
            public static void Is(Type arg, string name, Type constraint)
            {
                if (!constraint.IsAssignableFrom(arg))
                    throw new ArgumentException(
                        $"'{constraint}' or subtype of '{constraint}' expected, got '{arg}'.",
                        name);
            }

            public static void IsStruct(Type arg, string name)
            {
                if (!arg.IsValueType)
                    throw new ArgumentException($"'{arg}' is not a struct.", name);
            }

            public static void IsClass(Type arg, string name)
            {
                if (!arg.IsClass)
                    throw new ArgumentException($"'{arg}' is not a class.", name);
            }

            #endregion Type Constraints
        }
    }
}
