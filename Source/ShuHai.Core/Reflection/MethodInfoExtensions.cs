using System.Reflection;

namespace ShuHai.Reflection
{
    public static class MethodInfoExtensions
    {
        public static bool IsClosedConstructedMethod(this MethodInfo self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.IsGenericMethod && !self.ContainsGenericParameters;
        }

        public static bool IsPartialConstructedMethod(this MethodInfo self)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.IsGenericMethod && !self.IsGenericMethodDefinition && self.ContainsGenericParameters;
        }
    }
}
