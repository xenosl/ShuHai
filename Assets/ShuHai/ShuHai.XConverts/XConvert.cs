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

            var converter = settings.GetConverter(obj.GetType());
            return converter.ToXElement(obj, elementName, settings);
        }

        public static object ToObject(XElement element, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            return XConverter.Default.ToObject(element, settings);
        }
    }
}