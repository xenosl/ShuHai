using System;
using System.Collections.Generic;

namespace ShuHai.Unity
{
    using UObject = UnityEngine.Object;

    public static class UnityEnsure
    {
        public static class Argument
        {
            public static void NotNull<T>(T arg, string name)
                where T : UObject
            {
                if (!arg) // Equivalent to: arg != null && !arg.Equals(null) 
                    throw new ArgumentNullException(name);
            }

            public static void AllNotNull<T>(IEnumerable<T> arg, string name)
                where T : UObject
            {
                if (arg is IReadOnlyList<T> list)
                {
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (!list[i])
                            throw new ArgumentNullException($"{name}[{i}]");
                    }
                }
                else
                {
                    int i = 0;
                    foreach (var item in arg)
                    {
                        if (!item)
                            throw new ArgumentNullException($"{name}[{i}]");
                        i++;
                    }
                }
            }

            public static void AssetPath(string arg, string name)
            {
                if (!arg.StartsWith("Assets"))
                    throw new ArgumentException($"Asset path (starts with 'Assets') expected, got '{arg}'.", name);
            }
        }
    }
}
