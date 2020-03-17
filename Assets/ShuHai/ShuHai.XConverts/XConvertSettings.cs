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


        /// <summary>
        ///     Converters used during converts. <see cref="XConverter.BuiltIns" /> is used if the value is
        ///     <see langword="null" />.
        /// </summary>
        public ConverterCollection Converters;
    }
}