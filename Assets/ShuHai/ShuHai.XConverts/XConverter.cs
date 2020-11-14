using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
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
        public static XConverter Default { get; } = new XConverter();

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
            return ConvertType.IsValueType
                ? !ReferenceEquals(@object, null) && CanConvert(@object.GetType())
                : ReferenceEquals(@object, null) || CanConvert(@object.GetType());
        }

        public XElement ToXElement(object @object,
            string elementName, XConvertSettings settings = null, XConvertToXElementSession session = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));
            if (!CanConvert(@object))
                throw new ArgumentException("Unable to convert object to xml element.", nameof(@object));

            settings = settings ?? XConvertSettings.Default;
            if (ReferenceEquals(@object, null))
                return CreateNullXElement(elementName, settings.AssemblyNameStyle);

            session = session ?? new XConvertToXElementSession();
            if (session.TryGetID(@object, out var id))
                return CreateObjectReferenceXElement(elementName, id);

            var element = new XElement(elementName);
            session.AddElement(@object, element);
            var type = @object.GetType();
            if (!type.IsValueType)
                XConvert.WriteObjectID(element, session.GetOrGenerateID(@object));
            PopulateXElement(element, @object, settings, session);
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
        /// <param name="session">Session information of current convertion.</param>
        protected virtual void PopulateXElement(XElement element,
            object @object, XConvertSettings settings, XConvertToXElementSession session)
        {
            element.RemoveAll();
            PopulateXElementAttributes(element, @object, settings, session);
            PopulateXElementValue(element, @object, settings);
            PopulateXElementChildren(element, @object, settings);
        }

        protected virtual void PopulateXElementAttributes(XElement element,
            object @object, XConvertSettings settings, XConvertToXElementSession session)
        {
            XConvert.WriteObjectType(element, @object.GetType(), settings.AssemblyNameStyle);
        }

        protected virtual void PopulateXElementValue(XElement element, object @object, XConvertSettings settings) { }

        protected virtual void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            foreach (var member in XConvert.CollectConvertMembers(@object.GetType()))
                element.Add(member.ToXElement(@object, settings));
        }

        protected XElement CreateNullXElement(string elementName, FormatterAssemblyStyle? typeNameStyle)
        {
            var element = new XElement(elementName);
            XConvert.WriteObjectType(element, ConvertType.IsInstantiable() ? ConvertType : null, typeNameStyle);
            XConvert.WriteNullFlag(element, true);
            return element;
        }

        protected static XElement CreateObjectReferenceXElement(string elementName, long objectID)
        {
            var element = new XElement(elementName);
            XConvert.WriteObjectReference(element, objectID);
            return element;
        }

        #endregion Object To XElement

        #region XElement To Object

        public object ToObject(XElement element,
            XConvertSettings settings = null, XConvertToObjectSession session = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));

            var type = XConvert.ParseObjectType(element);
            if (type == null)
                return null;

            if (!CanConvert(type))
                throw new XmlException($"Can not convert specified xml element to '{type}' by {GetType()}.");

            if (XConvert.TryParseNullFlag(element, out var isNull) && isNull)
                return null;

            object @object;
            if (type.IsValueType)
            {
                @object = CreateObject(element, type, settings ?? XConvertSettings.Default);
                PopulateObjectMembers(@object, element, settings);
            }
            else
            {
                session = session ?? new XConvertToObjectSession();
                if (XConvert.TryParseObjectReference(element, out var objectID))
                    return session.GetObject(objectID);

                @object = CreateObject(element, type, settings ?? XConvertSettings.Default);
                session.AddObject(element, @object);
                PopulateObjectMembers(@object, element, settings);
            }
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

        #region Instances

        /// <summary>
        ///     A collection of built-in converters.
        /// </summary>
        public static IReadOnlyXConverterCollection BuiltIns { get; }
            = new XConverterCollection(DefaultConverters(typeof(XConverter).Assembly.ToEnumerable()));

        /// <summary>
        ///     A collection of all default instance of all instantiable converter types.
        /// </summary>
        /// <remarks>
        ///     The default instance here refers to the static property named 'Default' in certain converter class.
        ///     The instantiable type here indicates the type is not an abstract class.
        /// </remarks>
        public static IReadOnlyXConverterCollection Defaults { get; }
            = new XConverterCollection(DefaultConverters(Assemblies.Instances));

        private static IEnumerable<XConverter> DefaultConverters(IEnumerable<Assembly> searchAssemblies)
        {
            return InstantiableConverterTypes(searchAssemblies)
                .Select(DefaultInstanceOf)
                .Where(c => c != null);
        }

        private static IEnumerable<Type> InstantiableConverterTypes(IEnumerable<Assembly> searchAssemblies)
        {
            var rootType = typeof(XConverter);
            return searchAssemblies.SelectMany(a => a.GetTypes())
                .Where(t => t != rootType && !t.IsAbstract && rootType.IsAssignableFrom(t));
        }

        private static XConverter DefaultInstanceOf(Type type)
        {
            var property = type.GetProperty("Default", BindingFlags.Static | BindingFlags.Public);
            var converter = (XConverter)property?.GetValue(null);
            return converter != null && converter.GetType() == type ? converter : null;
        }

        #endregion Instances
    }
}