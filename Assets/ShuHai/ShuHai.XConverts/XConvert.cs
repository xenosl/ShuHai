using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;

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

            var converter = FindAppropriateConverter(settings, obj.GetType());
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

            var converter = FindAppropriateConverter(settings, type);
            return converter.ToObject(element, settings);
        }

        #endregion Convert

        #region Converter Select

        public static XConverter FindAppropriateConverter(XConvertSettings settings, Type type)
        {
            Ensure.Argument.NotNull(settings, nameof(settings));
            return FindAppropriateConverter(settings.Converters, type);
        }

        public static XConverter FindAppropriateConverter(IReadOnlyConverterCollection converters, Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));

            if (CollectionUtil.IsNullOrEmpty(converters))
                converters = XConverter.BuiltIns;

            if (XConvertSelector.TrySelect(converters, type, out var converter))
                return converter;
            return null;
        }

        internal static XConverter FindAppropriateConverter(
            IReadOnlyConverterCollection converters, object @object, Type fallbackType = null)
        {
            if (converters == null)
                converters = XConverter.BuiltIns;

            if (@object == null)
            {
                return fallbackType != null
                    ? FindAppropriateConverter(converters, fallbackType)
                    : XConverter.Default;
            }
            return FindAppropriateConverter(converters, @object.GetType());
        }

        #endregion Converter Select
    }
}