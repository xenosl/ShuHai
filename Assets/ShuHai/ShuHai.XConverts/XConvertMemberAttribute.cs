using System;

namespace ShuHai.XConverts
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class XConvertMemberAttribute : Attribute
    {
        public string Name { get; }

        public XConvertMemberAttribute() { }

        public XConvertMemberAttribute(string name) { Name = name; }
    }
}