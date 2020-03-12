namespace ShuHai.SConverts
{
    public abstract class SConvertOptions
    {
        public SConverterCollection Converters;
    }

    public sealed class SConvertToStringOptions : SConvertOptions
    {
        public static readonly SConvertToStringOptions Default = new SConvertToStringOptions();

        /// <summary>
        ///     The convert result for <see langword="null" /> value.
        /// </summary>
        public string StringForNullValue = string.Empty;
    }

    public sealed class SConvertToValueOptions : SConvertOptions
    {
        public static readonly SConvertToValueOptions Default = new SConvertToValueOptions();

        /// <summary>
        ///     The convert result for <see langword="null" /> target type.
        /// </summary>
        public object ObjectForNullType = null;
    }
}