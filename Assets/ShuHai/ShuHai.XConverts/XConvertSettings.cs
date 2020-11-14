using System;
using System.Runtime.Serialization.Formatters;

namespace ShuHai.XConverts
{
    public class XConvertSettings
    {
        public static XConvertSettings Default { get; } = new XConvertSettings();

        /// <summary>
        ///     Controls how assembly names are written during converting. A value of <see langword="null" /> means no assembly
        ///     name is written.
        /// </summary>
        public FormatterAssemblyStyle? AssemblyNameStyle { get; set; } = FormatterAssemblyStyle.Simple;

        /// <summary>
        ///     Converters used during converts.
        /// </summary>
        /// <remarks>
        ///     <see cref="XConverter.Defaults" /> is used if the value is <see langword="null" />.
        ///     Converters in <see cref="XConverter.BuiltIns" /> are always used no matter which converters are
        ///     included in the collection.
        /// </remarks>
        public XConverterCollection Converters { get; set; }

        public XConverter SelectConverter(object @object, Type fallbackType = null)
        {
            var type = @object?.GetType();
            if (type == null)
                type = fallbackType;
            return type == null ? XConverter.Default : SelectConverter(type);
        }

        public XConverter SelectConverter(Type convertType)
        {
            var collection = Converters ?? XConverter.Defaults;
            if (XConverterSelector.TrySelect(collection, convertType, out var converter))
                return converter;
            if (XConverterSelector.TrySelect(XConverter.BuiltIns, convertType, out converter))
                return converter;
            return XConverter.Default;
        }
    }
}