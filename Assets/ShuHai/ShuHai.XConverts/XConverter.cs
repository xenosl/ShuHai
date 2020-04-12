using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace ShuHai.XConverts
{
    /// <summary>
    ///     Root converter for types of converter.
    /// </summary>
    [XConvertType(typeof(object))]
    public class XConverter
    {
        public static readonly XConverter Default = new XConverter();

        protected XConverter() { ConvertType = GetConvertType(); }

        #region Convert Type

        /// <summary>
        ///     Base type of all types that can be converted by current converter.
        /// </summary>
        public Type ConvertType { get; }

        /// <summary>
        ///     Indicates the specified type of objects can be converted to a <see cref="XElement " /> or vice versa by
        ///     current instance.
        /// </summary>
        public bool CanConvert(Type type) { return CanConvert(ConvertType, type); }

        public static bool CanConvert(Type convertType, Type targetType)
        {
            if (!targetType.IsInstantiable())
                return false;

            return convertType.IsGenericTypeDefinition
                ? targetType.IsAssignableToConstructedGenericType(convertType)
                : convertType.IsAssignableFrom(targetType);
        }

        private Type GetConvertType()
        {
            var type = GetType();
            var attr = type.GetCustomAttribute<XConvertTypeAttribute>();
            if (attr == null)
                throw new MissingAttributeException($"Missing {typeof(XConvertTypeAttribute).Name} for {type}.");
            return attr.Type;
        }

        #endregion Convert Type

        #region Object To XElement

        /// <summary>
        ///     Indicates whether the specified object can be converted to a <see cref="XElement" /> instance.
        /// </summary>
        public virtual bool CanConvert(object @object)
        {
            if (ConvertType.IsValueType)
                return !ReferenceEquals(@object, null) && CanConvert(@object.GetType());
            return ReferenceEquals(@object, null) || CanConvert(@object.GetType());
        }

        public XElement ToXElement(object @object, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));
            XConvert.ArgOrDefault(ref settings);
            if (!CanConvert(@object))
                throw new ArgumentException("Unable to convert object to xml element.", nameof(@object));

            var element = new XElement(elementName);
            PopulateXElement(element, @object, settings ?? XConvertSettings.Default);
            return element;
        }

        /// <summary>
        ///     Populate contents of specified <see cref="XElement" /> instance from specified object.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> instance to populate.</param>
        /// <param name="object">The object that contains the contents to populate from.</param>
        /// <param name="settings">
        ///     Convert settings used when populating. <see cref="XConvertSettings.Default" /> will be used if the value is
        ///     <see langword="null" />.
        /// </param>
        protected virtual void PopulateXElement(XElement element, object @object, XConvertSettings settings)
        {
            element.RemoveAll();
            PopulateXAttributes(element, @object, settings);

            if (!ReferenceEquals(@object, null))
            {
                PopulateXElementValue(element, @object, settings);
                PopulateXElementChildren(element, @object, settings);
            }
        }

        protected virtual void PopulateXAttributes(XElement element, object @object, XConvertSettings settings)
        {
            XConvert.WriteObjectType(element, @object?.GetType(), settings.AssemblyNameStyle);
        }

        protected virtual void PopulateXElementValue(XElement element, object @object, XConvertSettings settings) { }

        protected virtual void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            foreach (var member in XConvert.CollectConvertMembers(type))
                element.Add(member.ToXElement(@object, settings));
        }

        #endregion Object To XElement

        #region XElement To Object

        public object ToObject(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            XConvert.ArgOrDefault(ref settings);

            var type = XConvert.ParseObjectType(element);
            if (type == null)
                return null;

            if (!CanConvert(type))
                throw new XmlException($"Can not convert specified xml element to '{type}' by {GetType()}.");

            var @object = CreateObject(element, type, settings);
            PopulateObjectMembers(@object, element, settings);
            return @object;
        }

        protected virtual object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return Activator.CreateInstance(type, true);
        }

        /// <summary>
        ///     Populate the contents of specified xml element to an object.
        /// </summary>
        /// <param name="object">The object to populate.</param>
        /// <param name="element">The xml element that contains the contents of the object to populate.</param>
        /// <param name="settings">
        ///     Convert settings used during the convert. <see cref="XConvertSettings.Default" /> is used if the value is
        ///     <see langword="null" />.
        /// </param>
        protected virtual void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var memberDict = XConvert.CollectConvertMembers(@object.GetType()).ToDictionary(m => m.Name);
            foreach (var childElement in element.Elements())
            {
                if (memberDict.TryGetValue(childElement.Name.LocalName, out var member))
                    member.SetValue(@object, childElement, settings);
            }
        }

        #endregion XElement To Object

        #region Built-in Instances

        /// <summary>
        ///     The collection that contains all default instances of all built-in <see cref="XConverter" />s.
        /// </summary>
        public static readonly IReadOnlyXConverterCollection BuiltIns;

        private static IReadOnlyXConverterCollection CollectBuiltIns()
        {
            var rootType = typeof(XConverter);
            var converters = rootType.Assembly.GetTypes()
                .Where(t => t != rootType && !t.IsAbstract && rootType.IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<XConverter>();

            var builtIns = new XConverterCollection { Default };
            builtIns.AddRange(converters);
            return builtIns;
        }

        #endregion Built-in Instances

        static XConverter() { BuiltIns = CollectBuiltIns(); }
    }
}