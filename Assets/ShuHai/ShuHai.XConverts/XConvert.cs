using System.Xml.Linq;

namespace ShuHai.XConverts
{
    public static class XConvert
    {
        #region Convert

        public static XElement ToXElement(object obj, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));

            if (settings == null)
                settings = XConvertSettings.Default;

            var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, obj);
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

            var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, type);
            return converter.ToObject(element, settings);
        }

        #endregion Convert
    }
}