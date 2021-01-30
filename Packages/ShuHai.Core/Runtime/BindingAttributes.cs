using System.Reflection;

namespace ShuHai
{
    /// <summary>
    ///     Provides shortcuts of frequently used <see cref="BindingFlags" /> combinations.
    /// </summary>
    public static class BindingAttributes
    {
        public const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DeclareInstance = BindingFlags.DeclaredOnly
            | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DeclareStatic = BindingFlags.DeclaredOnly
            | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DeclareAll = BindingFlags.DeclaredOnly
            | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    }
}