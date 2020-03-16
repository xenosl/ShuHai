using System.Xml.Linq;

namespace ShuHai.XConverts
{
    public static class XConvert
    {
        public static XElement ToXElement(object obj, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));

            if (settings == null)
                settings = XConvertSettings.Default;

            var converter = settings.FindAppropriateConverter(obj.GetType());
            return converter.ToXElement(obj, elementName, settings);
        }

        public static object ToObject(XElement element, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(element, nameof(element));

            if (settings == null)
                settings = XConvertSettings.Default;

            var type = XConverter.ParseObjectType(element);
            if (type == null)
                return null;

            var converter = settings.FindAppropriateConverter(type);
            return converter.ToObject(element, settings);
        }
    }
}