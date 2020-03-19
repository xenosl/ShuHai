using System;
using System.Collections.Generic;

namespace ShuHai.XConverts
{
    public static class XConvertSelector
    {
        public static bool TrySelect<T>(IReadOnlyDictionary<Type, T> collection, Type convertType, out T item)
        {
            Ensure.Argument.NotNull(convertType, nameof(convertType));

            item = default;
            if (CollectionUtil.IsNullOrEmpty(collection))
                return false;

            if (FindInType(collection, convertType, out item))
                return true;
            if (FindInInterfaces(collection, convertType, out item))
                return true;
            if (FindInBaseTypes(collection, convertType, out item))
                return true;
            return false;
        }

        private static bool FindInType<T>(IReadOnlyDictionary<Type, T> collection, Type type, out T item)
        {
            return type.IsGenericType
                ? collection.TryGetValue(type.GetGenericTypeDefinition(), out item)
                : collection.TryGetValue(type, out item);
        }

        private static bool FindInBaseTypes<T>(IReadOnlyDictionary<Type, T> collection, Type type, out T item)
        {
            var p = type.BaseType;
            while (p != null)
            {
                if (FindInType(collection, p, out item))
                    return true;
                p = p.BaseType;
            }
            item = default;
            return false;
        }

        private static bool FindInInterfaces<T>(IReadOnlyDictionary<Type, T> collection, Type type, out T item)
        {
            var interfaces = type.GetMostDerivedInterfaces();
            foreach (var @interface in interfaces)
            {
                if (FindInType(collection, @interface, out item))
                    return true;
            }

            foreach (var @interface in interfaces)
            {
                if (FindInInterfaces(collection, @interface, out item))
                    return true;
            }

            item = default;
            return false;
        }
    }
}