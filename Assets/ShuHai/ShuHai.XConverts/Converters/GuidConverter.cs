using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(Guid))]
    public class GuidConverter : ValueConverter
    {
        public new static GuidConverter Default { get; } = new GuidConverter();

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return Guid.Parse(element.Value);
        }
    }
}