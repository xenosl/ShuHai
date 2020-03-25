using System.Reflection;

namespace ShuHai
{
    public static class BindingAttributes
    {
        public const BindingFlags DeclareInstance = BindingFlags.DeclaredOnly
            | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DeclareStatic = BindingFlags.DeclaredOnly
            | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags DeclareAll = BindingFlags.DeclaredOnly
            | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    }
}