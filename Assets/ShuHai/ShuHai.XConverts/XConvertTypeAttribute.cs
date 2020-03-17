using System;

namespace ShuHai.XConverts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XConvertTypeAttribute : Attribute
    {
        public Type Type;

        public int Priority;

        public XConvertTypeAttribute(Type type, int priority = 0)
        {
            Ensure.Argument.NotNull(type, nameof(type));

            Type = type;
            Priority = priority;
        }
    }
}