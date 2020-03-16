using System;
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

        /// <summary>
        ///     Converters used during converts. <see cref="XConverter.BuiltIns" /> is used if the value is <see langword="null"/>.
        /// </summary>
        public ConverterCollection Converters;

        public XConverter FindAppropriateConverter(Type targetType)
        {
            return (Converters ?? XConverter.BuiltIns).FindAppropriateConverter(targetType);
        }

        internal XConverter FindAppropriateConverter(object @object, Type fallbackType = null)
        {
            var converters = Converters ?? XConverter.BuiltIns;
            if (@object == null)
                return fallbackType != null ? converters.FindAppropriateConverter(fallbackType) : XConverter.Default;
            return converters.FindAppropriateConverter(@object.GetType());
        }

        #endregion Converters
    }
}