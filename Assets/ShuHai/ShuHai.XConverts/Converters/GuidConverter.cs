using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(Guid))]
    public class GuidConverter : XConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            element.Value = ((Guid)@object).ToString();
        }

        protected override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            // Nothing to do...
        }

        protected override object CreateObject(XElement element, Type type) { return Guid.Parse(element.Value); }

        protected override void PopulateObjectMembersImpl(object @object, XElement element, XConvertSettings settings)
        {
            // Nothing to do...
        }
    }
}