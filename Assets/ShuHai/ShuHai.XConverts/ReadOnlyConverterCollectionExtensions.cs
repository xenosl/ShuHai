using System;

namespace ShuHai.XConverts
{
    public static class ReadOnlyConverterCollectionExtensions
    {
        public static XConverter FindAppropriateConverter(this IReadOnlyConverterCollection self, Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));

            if (CollectionUtil.IsNullOrEmpty(self))
                return null;

            if (FindInType(self, type, out var converter))
                return converter;
            if (FindInInterfaces(self, type, out converter))
                return converter;
            if (FindInBaseTypes(self, type, out converter))
                return converter;
            return null;

//            return self.Where(c => c.CanConvert(type)) // All available converters
//                .OrderByDescending(c => type.GetDeriveDepth(c.ConvertType))
//                .FirstOrDefault();
        }

        private static bool FindInType(IReadOnlyConverterCollection collection, Type type, out XConverter converter)
        {
            return type.IsGenericType
                ? collection.TryGet(type.GetGenericTypeDefinition(), out converter)
                : collection.TryGet(type, out converter);
        }

        private static bool FindInBaseTypes(
            IReadOnlyConverterCollection collection, Type type, out XConverter converter)
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
            IReadOnlyConverterCollection collection, Type type, out XConverter converter)
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