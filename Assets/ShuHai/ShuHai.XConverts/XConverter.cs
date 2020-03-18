using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ShuHai.XConverts
{
    /// <summary>
    ///     Root converter for types of converter.
    /// </summary>
    [XConvertType(typeof(object))]
    public class XConverter
    {
        /// <summary>
        ///     Name of the attribute that sepcifies the type of an object.
        /// </summary>
        public const string TypeAttributeName = "Type";

        /// <summary>
        ///     Attribute name for <see langword="null" /> type.
        /// </summary>
        public const string NullTypeName = "$null";

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

        protected void EnsureArgumentConvertible(object @object)
        {
            if (!CanConvert(@object))
                throw new ArgumentException("Unable to convert object to xml element.", nameof(@object));
        }

        public XElement ToXElement(object @object, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));
            EnsureArgumentConvertible(@object);

            var element = new XElement(elementName);
            PopulateXElementImpl(element, @object, settings ?? XConvertSettings.Default);
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
        public void PopulateXElement(XElement element, object @object, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            EnsureArgumentConvertible(@object);

            PopulateXElementImpl(element, @object, settings ?? XConvertSettings.Default);
        }

        protected virtual void PopulateXElementImpl(XElement element, object @object, XConvertSettings settings)
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
            WriteObjectType(element, @object?.GetType(), settings.AssemblyNameStyle);
        }

        protected virtual void PopulateXElementValue(XElement element, object @object, XConvertSettings settings) { }

        protected virtual void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            foreach (var field in EnumerateConvertibleFields(type))
            {
                var value = field.GetValue(@object);
                var converter = XConvert.FindAppropriateConverter(settings.Converters, value, field.FieldType);
                var fieldXElem = converter.ToXElement(value, field.Name, settings);
                element.Add(fieldXElem);
            }
        }

        #endregion Object To XElement

        #region XElement To Object

        public object ToObject(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            return ToObjectImpl(element, settings ?? XConvertSettings.Default);
        }

        protected virtual object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return Activator.CreateInstance(type);
        }

        protected virtual object ToObjectImpl(XElement element, XConvertSettings settings)
        {
            var type = ParseObjectType(element);
            if (type == null)
                return null;

            if (!CanConvert(type))
                throw new XmlException($"Can not convert specified xml element to '{type}' by {GetType()}.");

            var @object = CreateObject(element, type, settings);
            PopulateObjectMembersImpl(@object, element, settings);
            return @object;
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
        public void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));
            PopulateObjectMembersImpl(@object, element, settings ?? XConvertSettings.Default);
        }

        protected virtual void PopulateObjectMembersImpl(object @object, XElement element, XConvertSettings settings)
        {
            var fieldDict = EnumerateConvertibleFields(@object.GetType()).ToDictionary(f => f.Name);
            foreach (var childElement in element.Elements())
            {
                if (!fieldDict.TryGetValue(childElement.Name.LocalName, out var field))
                    continue;

                var fieldType = ParseObjectType(childElement);
                if (fieldType == null)
                    throw new XmlException($"Type attribute not found for field '{field.Name}'.");

                if (!field.FieldType.IsAssignableFrom(fieldType))
                    continue;

                var converter = XConvert.FindAppropriateConverter(settings.Converters, fieldType);
                field.SetValue(@object, converter.ToObject(childElement, settings));
            }
        }

        #endregion XElement To Object

        #region Built-in Instances

        /// <summary>
        ///     The collection that contains all default instances of all built-in <see cref="XConverter" />s.
        /// </summary>
        public static readonly IReadOnlyConverterCollection BuiltIns;

        private static IReadOnlyConverterCollection InitializeBuiltIns()
        {
            var rootType = typeof(XConverter);
            var converters = rootType.Assembly.GetTypes()
                .Where(t => t != rootType && !t.IsAbstract && rootType.IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<XConverter>();

            var builtIns = new ConverterCollection { Default };
            builtIns.AddRange(converters);
            return builtIns;
        }

        #endregion Built-in Instances

        #region Utilities

        public static void WriteObjectType(XElement element, Type type,
            FormatterAssemblyStyle? style = FormatterAssemblyStyle.Simple)
        {
            var typeName = type == null ? NullTypeName : TypeName.Get(type).ToString(style);
            var attr = element.Attribute(TypeAttributeName);
            if (attr == null)
                element.Add(new XAttribute(TypeAttributeName, typeName));
            else
                attr.Value = typeName;
        }

        public static Type ParseObjectType(XElement element)
        {
            var typeAttr = element.Attribute(TypeAttributeName);
            if (typeAttr == null)
                return null;
            if (typeAttr.Value == NullTypeName)
                return null;

            var type = TypeCache.GetType(typeAttr.Value);
            if (type == null)
                throw new XmlException($@"Failed to load type ""{typeAttr.Value}"".");

            return type;
        }

        public static IEnumerable<FieldInfo> EnumerateConvertibleFields(Type type)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //return fields.Where(f => f.IsDefined(typeof(XConvertibleAttribute)));
            return fields.Where(f => !f.IsDefined(typeof(XmlIgnoreAttribute)));
        }

        #endregion Utilities

        static XConverter() { BuiltIns = InitializeBuiltIns(); }
    }
}