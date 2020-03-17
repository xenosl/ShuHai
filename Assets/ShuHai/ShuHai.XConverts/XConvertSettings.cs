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
            return XConvertUtilities.FindAppropriateConverter(Converters ?? XConverter.BuiltIns, targetType);
        }

        internal XConverter FindAppropriateConverter(object @object, Type fallbackType = null)
        {
            var converters = Converters ?? XConverter.BuiltIns;
            if (@object == null)
            {
                return fallbackType != null
                    ? XConvertUtilities.FindAppropriateConverter(converters, fallbackType)
                    : XConverter.Default;
            }
            return XConvertUtilities.FindAppropriateConverter(converters, @object.GetType());
        }

        #endregion Converters
    }
}