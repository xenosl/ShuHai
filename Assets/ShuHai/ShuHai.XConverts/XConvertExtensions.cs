using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ShuHai.XConverts
{
    internal static class XConvertExtensions
    {
        public static XConverter FindAppropriateConverter(
            this IReadOnlyDictionary<Type, XConverter> self, Type targetType)
        {
            XConverter converter = null;
            if (targetType.IsValueType)
            {
                converter = self[targetType];
            }
            else
            {
                var type = targetType;
                while (type != null)
                {
                    converter = self[type];
                    if (converter != null)
                        break;
                    type = type.BaseType;
                }
            }
            return converter;
        }

        public static IEnumerable<XConverter> FindAvailableConverters(
            this IReadOnlyDictionary<Type, XConverter> self, Type targetType)
        {
            if (targetType.IsValueType)
            {
                if (self.TryGetValue(targetType, out var converter))
                    yield return converter;
                yield break;
            }

            var type = targetType;
            while (type != null)
            {
                if (self.TryGetValue(type, out var converter))
                    yield return converter;
                type = type.BaseType;
            }

            var interfaceTypes = type.GetMostDerivedInterfaces();
        }
    }
}