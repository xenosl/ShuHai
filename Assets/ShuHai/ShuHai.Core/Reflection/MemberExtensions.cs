using System;
using System.Reflection;

namespace ShuHai.Reflection
{
    public static class MemberExtensions
    {
        public static bool IsStatic(this MemberInfo self)
        {
            switch (self)
            {
                case FieldInfo field:
                    return field.IsStatic;
                case PropertyInfo prop:
                    return prop.IsStatic();
                case MethodInfo method:
                    return method.IsStatic;
                case EventInfo evt:
                    return evt.IsStatic();
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool IsStatic(this PropertyInfo self)
        {
            return (self.GetMethod?.IsStatic ?? self.SetMethod?.IsStatic).Value;
        }

        public static bool IsStatic(this EventInfo self)
        {
            return (self.AddMethod?.IsStatic ?? self.RemoveMethod?.IsStatic).Value;
        }
    }
}