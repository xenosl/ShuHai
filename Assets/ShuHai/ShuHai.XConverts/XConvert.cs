using System;
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

            if (FindInType(converters, type, out var converter))
                return converter;
            if (FindInInterfaces(converters, type, out converter))
                return converter;
            if (FindInBaseTypes(converters, type, out converter))
                return converter;
            return null;

//            return self.Where(c => c.CanConvert(type)) // All available converters
//                .OrderByDescending(c => type.GetDeriveDepth(c.ConvertType))
//                .FirstOrDefault();
        }

        private static bool FindInType(IReadOnlyConverterCollection converters, Type type, out XConverter converter)
        {
            return type.IsGenericType
                ? converters.TryGet(type.GetGenericTypeDefinition(), out converter)
                : converters.TryGet(type, out converter);
        }

        private static bool FindInBaseTypes(
            IReadOnlyConverterCollection converters, Type type, out XConverter converter)
        {
            var p = type.BaseType;
            while (p != null)
            {
                if (FindInType(converters, p, out converter))
                    return true;
                p = p.BaseType;
            }
            converter = null;
            return false;
        }

        private static bool FindInInterfaces(
            IReadOnlyConverterCollection converters, Type type, out XConverter converter)
        {
            var interfaces = type.GetMostDerivedInterfaces();
            foreach (var @interface in interfaces)
            {
                if (FindInType(converters, @interface, out converter))
                    return true;
            }

            foreach (var @interface in interfaces)
            {
                if (FindInInterfaces(converters, @interface, out converter))
                    return true;
            }

            converter = null;
            return false;
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