using System;

namespace ShuHai.Edit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectDataConvertTypeAttribute : Attribute
    {
        public Type ConvertType { get; }

        public ObjectDataConvertTypeAttribute(Type convertType) { ConvertType = convertType; }
    }
}