using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(string))]
    internal sealed class StringConverter : ValueConverter
    {
        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return type == null ? null : element.Value;
        }
    }
}