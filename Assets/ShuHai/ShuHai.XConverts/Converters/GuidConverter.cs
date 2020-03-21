using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(Guid))]
    public class GuidConverter : ValueConverter
    {
        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return Guid.Parse(element.Value);
        }
    }
}