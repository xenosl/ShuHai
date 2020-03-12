using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;

namespace ShuHai.XConverts
{
    public class XConvertSettings
    {
        public static readonly XConvertSettings Default = new XConvertSettings();

        /// <summary>
        ///     Controls how assembly names are written during converting. A value of <see langword="null" /> means no assembly
        ///     name is written.
        /// </summary>
        public FormatterAssemblyStyle? AssemblyNameStyle = FormatterAssemblyStyle.Simple;

        #region Converters

        public ConverterCollection Converters = new ConverterCollection(XConverter.BuiltIns);

        /// <summary>
        ///     Get appropriate <see cref="XConverter" /> for specified <see cref="Type" /> of object.
        /// </summary>
        /// <param name="targetType">Type of the object to convert.</param>
        /// <returns>The most appropriate <see cref="XConverter" /> instance for object of type <paramref name="targetType" />.</returns>
        public XConverter GetConverter(Type targetType)
        {
            Ensure.Argument.NotNull(targetType, nameof(targetType));

            if (targetType.IsInterface)
                throw new NotSupportedException("Get converter for interfaces is not supported for now.");

            if (targetType.IsPrimitive)
                return XConverter.Default;

            if (CollectionUtil.IsNullOrEmpty(Converters))
                return null;

            XConverter converter = null;
            if (targetType.IsValueType)
            {
                converter = Converters[targetType];
            }
            else
            {
                var type = targetType;
                while (type != null)
                {
                    converter = Converters[type];
                    if (converter != null)
                        break;
                    type = type.BaseType;
                }
            }

            if (converter == null)
                converter = XConverter.BuiltIns[targetType];

            return converter;
        }

        #endregion Converters
    }
}