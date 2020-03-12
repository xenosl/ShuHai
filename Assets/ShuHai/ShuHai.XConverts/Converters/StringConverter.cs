using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(String))]
    internal sealed class StringConverter : XConverter
    {
        protected sealed override void PopulateXElementValue(
            XElement element, object @object, XConvertSettings settings)
        {
            element.Value = @object != null ? (string)@object : string.Empty;
        }

        protected sealed override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            throw new NotSupportedException("Populate child elements of a string is not supported.");
        }

        protected override object ToObjectImpl(XElement element, XConvertSettings settings)
        {
            var type = ParseObjectType(element);
            return type == null ? null : element.Value;
        }
    }
}