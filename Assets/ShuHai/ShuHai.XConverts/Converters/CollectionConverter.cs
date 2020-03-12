using System.Collections;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(IEnumerable))]
    public class CollectionConverter : XConverter
    {
        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
        }
    }
}