using System;

namespace ShuHai
{
    public class TargetTypesAttribute : Attribute
    {
        public Type[] Types;

        public TargetTypesAttribute(params Type[] types) { Types = types; }
    }
}