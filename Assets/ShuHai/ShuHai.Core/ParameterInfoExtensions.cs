using System.Reflection;

namespace ShuHai
{
    public enum ParameterModifier
    {
        None,
        Ref,
        Out
    }

    public static class ParameterInfoExtensions
    {
        public static ParameterModifier GetReferenceModifier(this ParameterInfo self)
        {
            var type = self.ParameterType;
            if (!type.IsByRef)
                return ParameterModifier.None;
            return self.IsOut ? ParameterModifier.Out : ParameterModifier.Ref;
        }
    }
}