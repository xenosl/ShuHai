using System.Runtime.Serialization.Formatters;

namespace ShuHai.XConverts
{
    public enum ValueStyle { Text, Byte }

    public class XConvertSettings
    {
        public static readonly XConvertSettings Default = new XConvertSettings();

        /// <summary>
        ///     Controls how assembly names are written during converting. A value of <see langword="null" /> means no assembly
        ///     name is written.
        /// </summary>
        public FormatterAssemblyStyle? AssemblyNameStyle = FormatterAssemblyStyle.Simple;

        /// <summary>
        ///     Specifies which format is used when convert float values. To keep precision of floating-point values,
        ///     <see cref="ValueStyle.Byte" /> is recommended.
        /// </summary>
        public ValueStyle FloatingPointStyle = ValueStyle.Text;

        /// <summary>
        ///     Converters used during converts. <see cref="XConverter.BuiltIns" /> is used if the value is
        ///     <see langword="null" />.
        /// </summary>
        public ConverterCollection Converters;
    }
}