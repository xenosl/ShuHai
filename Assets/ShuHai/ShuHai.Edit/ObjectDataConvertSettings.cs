namespace ShuHai.Edit
{
    public sealed class ObjectDataConvertSettings
    {
        public static ObjectDataConvertSettings Default { get; } = new ObjectDataConvertSettings();

        public ObjectDataConverterCollection Converters { get; set; } = ObjectDataConverterCollection.Default;

        public ObjectDataConverterSelector Selector { get; set; } = ObjectDataConverterSelector.Default;

        /// <summary>
        ///     Whether to ignore the constructor with single parameter of type <see cref="ObjectData" /> when converting.
        /// </summary>
        /// <remarks>
        ///     <para>The <see cref="ObjectDataConverter" /> converts an object in two approaches.</para>
        ///     <para>
        ///         1. Create the object via the default constructor which has no parameter, then populate contents of the
        ///         object from members of <see cref="ObjectData"/>.
        ///     </para>
        ///     <para>
        ///         2. Create the object via the data constructor which accepts a parameter of type <see cref="ObjectData"/>,
        ///         the object performs its construction with the data parameter.
        ///     </para>
        ///     <para>
        ///         The second approach is preferred if the data constructor exists, otherwise the first. The option changes
        ///         the behaviour, makes the first approach selected while both approaches are available.
        ///     </para>
        /// </remarks>
        public bool IgnoreDataConstructor { get; set; } = false;
    }
}