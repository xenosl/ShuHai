using System;

namespace ShuHai.XConverts
{
    public static class XConverterSelector
    {
        public static XConverter SelectWithBuiltins(
            IReadOnlyXConverterCollection collection, object @object, Type fallbackType = null)
        {
            var type = @object?.GetType();
            if (type == null)
                type = fallbackType;
            return type == null ? XConverter.Default : SelectWithBuiltins(collection, type);
        }

        /// <summary>
        ///     Select the most appropriate converter for the specified convert type from the specified converter collection.
        ///     The <see cref="XConverter.BuiltIns" /> is used if the specified converter collection is empty.
        /// </summary>
        public static XConverter SelectWithBuiltins(
            IReadOnlyXConverterCollection collection, Type convertType)
        {
            if (TrySelect(collection, convertType, out var converter))
                return converter;
            if (TrySelect(XConverter.BuiltIns, convertType, out converter))
                return converter;
            return XConverter.Default;
        }

        /// <summary>
        ///     Select the most appropriate converter for the specified convert type from the specified converter collection.
        /// </summary>
        public static bool TrySelect(
            IReadOnlyXConverterCollection collection, Type convertType, out XConverter converter)
        {
            Ensure.Argument.NotNull(convertType, nameof(convertType));

            converter = null;
            if (CollectionUtil.IsNullOrEmpty(collection))
                return false;

            if (FindInType(collection, convertType, out converter))
                return true;
            if (FindInInterfaces(collection, convertType, out converter))
                return true;
            if (FindInBaseTypes(collection, convertType, out converter))
                return true;
            return false;
        }

        private static bool FindInType(
            IReadOnlyXConverterCollection collection, Type type, out XConverter converter)
        {
            return type.IsGenericType
                ? collection.TryGetValue(type.GetGenericTypeDefinition(), out converter)
                : collection.TryGetValue(type, out converter);
        }

        private static bool FindInBaseTypes(
            IReadOnlyXConverterCollection collection, Type type, out XConverter converter)
        {
            var p = type.BaseType;
            while (p != null)
            {
                if (FindInType(collection, p, out converter))
                    return true;
                p = p.BaseType;
            }

            converter = null;
            return false;
        }

        private static bool FindInInterfaces(
            IReadOnlyXConverterCollection collection, Type type, out XConverter converter)
        {
            var interfaces = type.GetMostDerivedInterfaces();
            foreach (var @interface in interfaces)
            {
                if (FindInType(collection, @interface, out converter))
                    return true;
            }

            foreach (var @interface in interfaces)
            {
                if (FindInInterfaces(collection, @interface, out converter))
                    return true;
            }

            converter = null;
            return false;
        }
    }
}