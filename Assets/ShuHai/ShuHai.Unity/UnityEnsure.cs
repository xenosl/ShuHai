using System;

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
        }
    }
}