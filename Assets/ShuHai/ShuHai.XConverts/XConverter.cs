using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ShuHai.XConverts
{
    /// <summary>
    ///     Root converter for types of converter.
    /// </summary>
    public class XConverter
    {
        public const string TypeAttributeName = "Type";
        public const string NullTypeName = "$null";

        public static readonly XConverter Default = new XConverter();

        public virtual Type BaseConvertType { get; } = typeof(object);

        /// <summary>
        ///     Indicates the specified type of objects can be converted to a <see cref="XElement " /> or vice versa by current
        ///     instance.
        /// </summary>
        public virtual bool CanConvert(Type type)
        {
            return type.IsInstantiable() && BaseConvertType.IsAssignableFrom(type);
        }

        #region Object To XElement

        /// <summary>
        ///     Indicates whether the specified object can be converted to a <see cref="XElement" /> instance.
        /// </summary>
        public virtual bool CanConvert(object @object)
        {
            if (BaseConvertType.IsValueType)
                return !ReferenceEquals(@object, null);
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
            var typeName = @object == null
                ? NullTypeName
                : TypeName.Get(@object.GetType()).ToString(settings.AssemblyNameStyle);
            element.Add(new XAttribute(TypeAttributeName, typeName));
        }

        protected virtual void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            if (type.IsPrimitive)
                element.Value = @object.ToString();
        }

        protected virtual void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            if (type.IsPrimitive)
                return;

            foreach (var field in EnumerateConvertibleFields(type))
            {
                var value = field.GetValue(@object);
                var converter = value != null
                    ? settings.GetConverter(value.GetType())
                    : settings.GetConverter(field.FieldType);
                var fieldXElem = converter.ToXElement(value, field.Name, settings);
                element.Add(fieldXElem);
            }
        }

        #endregion Object To XElement

        #region XElement To Object

        //public virtual bool CanConvert(XElement element) { return true; }

        public object ToObject(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            return ToObjectImpl(element, settings ?? XConvertSettings.Default);
        }

        protected virtual object ToObjectImpl(XElement element, XConvertSettings settings)
        {
            var type = ParseObjectType(element);
            if (type == null)
                return null;

            if (type.IsPrimitive)
                return Convert.ChangeType(element.Value, type);

            if (!CanConvert(type))
                throw new XmlException($"Can not convert specified xml element to '{type}' by {GetType()}.");

            var @object = Activator.CreateInstance(type);
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

            if (@object.GetType().IsPrimitive)
                throw new ArgumentException("Attempt to populate members of primitive type.", nameof(@object));

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

                var converter = settings.GetConverter(fieldType);
                field.SetValue(@object, converter.ToObject(childElement, settings));
            }
        }

        #endregion XElement To Object

        #region Built-in Instances

        /// <summary>
        ///     The collection that contains all built-in <see cref="XConverter" /> instances.
        /// </summary>
        public static IReadOnlyConverterCollection BuiltIns => builtIns;

        private static readonly ConverterCollection builtIns = new ConverterCollection();

        private static void InitializeBuiltIns()
        {
            var rootType = typeof(XConverter);
            var converters = rootType.Assembly.GetTypes()
                .Where(t => t != rootType && !t.IsAbstract && rootType.IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<XConverter>();
            builtIns.Add(Default);
            builtIns.AddRange(converters);
        }

        #endregion Built-in Instances

        #region Utilities

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

        static XConverter() { InitializeBuiltIns(); }
    }
}