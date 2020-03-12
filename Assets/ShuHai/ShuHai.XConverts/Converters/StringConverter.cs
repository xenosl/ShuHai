using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(String))]
    internal sealed class StringConverter : XConverter
    {
        protected override void PopulateXElementImpl(XElement element, object obj, XConvertSettings settings)
        {
            element.RemoveAll();
            PopulateXAttributes(element, obj, settings);
            element.Value = obj != null ? (string)obj : string.Empty;
        }

        protected override object ToObjectImpl(XElement element, XConvertSettings settings)
        {
//            var type = ParseObjectTypeForObjectCreation(element);
//            return type == null ? null : element.Value;
            throw new NotImplementedException();
        }
    }
}