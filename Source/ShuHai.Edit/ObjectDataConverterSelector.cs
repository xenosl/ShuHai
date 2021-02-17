using System;

namespace ShuHai.Edit
{
    public class ObjectDataConverterSelector
    {
        public static ObjectDataConverterSelector Default { get; } = new ObjectDataConverterSelector();

        public virtual ObjectDataConverter Select(IReadOnlyObjectDataConverterCollection collection, Type convertType)
        {
            Ensure.Argument.NotNull(collection, nameof(collection));
            if (collection.Count == 0)
                return null;

            ObjectDataConverter converter;
            if (FindInType(collection, convertType, out converter))
                return converter;
            if (FindInInterfaces(collection, convertType, out converter))
                return converter;
            if (FindInBaseTypes(collection, convertType, out converter))
                return converter;
            return null;
        }

        private static bool FindInType(
            IReadOnlyObjectDataConverterCollection collection, Type type, out ObjectDataConverter converter)
        {
            return type.IsGenericType
                ? collection.TryGetConverter(type.GetGenericTypeDefinition(), out converter)
                : collection.TryGetConverter(type, out converter);
        }

        private static bool FindInBaseTypes(
            IReadOnlyObjectDataConverterCollection collection, Type type, out ObjectDataConverter converter)
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
            IReadOnlyObjectDataConverterCollection collection, Type type, out ObjectDataConverter converter)
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