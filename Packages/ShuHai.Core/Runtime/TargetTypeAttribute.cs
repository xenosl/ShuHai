using System;

namespace ShuHai
{
    public class TargetTypeAttribute : Attribute
    {
        public Type Type;

        public TargetTypeAttribute(Type type) { Type = type; }
    }
}