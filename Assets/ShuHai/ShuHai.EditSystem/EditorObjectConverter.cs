using System;
using System.Xml.Linq;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    [XConvertType(typeof(EditorObject))]
    public class EditorObjectConverter : XConverter
    {
        private static class MemberNames
        {
            public const string Value = "Value";
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var value = XConvert.ToObject(element.Element(MemberNames.Value), settings);
            return Activator.CreateInstance(type,
                BindingAttributes.DeclareInstance, null, new[] { value }, null);
        }
    }
}