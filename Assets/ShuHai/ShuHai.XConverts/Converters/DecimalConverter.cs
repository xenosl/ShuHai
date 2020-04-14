using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(decimal))]
    public class DecimalConverter : ValueConverter
    {
        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return decimal.Parse(element.Value);
        }
    }
}