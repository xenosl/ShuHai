using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Xml;
using System.Xml.Linq;
using ShuHai.Reflection;

namespace ShuHai.XConverts
{
    public static class XConvert
    {
        #region Convert

        public static bool CanConvert(MemberInfo member)
        {
            Ensure.Argument.NotNull(member, nameof(member));

            var mt = member.MemberType;
            if (mt != MemberTypes.Property && mt != MemberTypes.Field)
                return false;

            if (mt == MemberTypes.Field)
            {
                var field = (FieldInfo)member;
                if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                    return false;
            }

            if (mt == MemberTypes.Property)
            {
                var prop = (PropertyInfo)member;
                if (prop.SetMethod == null || prop.GetMethod == null)
                    return false;
                if (typeof(Delegate).IsAssignableFrom(prop.PropertyType))
                    return false;
            }

            if (!IsValidXElementName(XElementNameOf(member)))
                return false;

            if (member.IsDefined(typeof(CompilerGeneratedAttribute)))
                return false;
            return !member.IsDefined(typeof(XConvertIgnoreAttribute));
        }

        #region Object To XElement

        public static XElement ToXElement(object @object, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));
            ArgOrDefault(ref settings);

            return ToXElementImpl(@object, null, elementName, settings);
        }

        public static XElement ToXElement(Type type, string memberName,
            BindingFlags bindingAttr, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.NotNullOrEmpty(memberName, nameof(memberName));

            var am = AssignableMember.Get(type, memberName, bindingAttr);
            if (am == null)
                throw new MissingMemberException($"Member '{memberName}' of type '{type}' not found.");

            if (!am.IsStatic)
                Ensure.Argument.NotNull(target, nameof(target));

            return ToXElement(am.Info, target, settings);
        }

        public static XElement ToXElement(MemberInfo member, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(member, nameof(member));

            switch (member)
            {
                case FieldInfo field:
                    return ToXElement(field, target, settings);
                case PropertyInfo property:
                    return ToXElement(property, target, settings);
                default:
                    throw new ArgumentException($"Attempt to convert {member.MemberType} to XElement.", nameof(member));
            }
        }

        public static XElement ToXElement(FieldInfo field, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(field, nameof(field));
            if (!field.IsStatic)
                Ensure.Argument.NotNull(target, nameof(target));
            ArgOrDefault(ref settings);

            var value = field.GetValue(target);
            return ToXElementImpl(value, field.FieldType, XElementNameOf(field), settings);
        }

        public static XElement ToXElement(PropertyInfo property, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(property, nameof(property));
            var getMethod = property.GetMethod;
            Ensure.Argument.NotNull(getMethod, nameof(property.GetMethod));
            if (!getMethod.IsStatic)
                Ensure.Argument.NotNull(target, nameof(target));
            ArgOrDefault(ref settings);

            var value = property.GetValue(target);
            return ToXElementImpl(value, property.PropertyType, XElementNameOf(property), settings);
        }

        private static XElement ToXElementImpl(
            object @object, Type fallbackType, string elementName, XConvertSettings settings)
        {
            var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, @object, fallbackType);
            return converter.ToXElement(@object, elementName, settings);
        }

        #endregion Object To XElement

        #region XElement To Object

        public static object ToObject(XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            ArgOrDefault(ref settings);

            var type = ParseObjectType(element);
            if (type == null)
                return null;

            var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, type);
            return converter.ToObject(element, settings);
        }

        public static bool SetValue(this MemberInfo member,
            object target, XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(member, nameof(member));

            switch (member)
            {
                case FieldInfo field:
                    return SetValue(field, target, element, settings);
                case PropertyInfo property:
                    return SetValue(property, target, element, settings);
                default:
                    throw new ArgumentException($"Attempt to set value of {member.MemberType}.", nameof(member));
            }
        }

        public static bool SetValue(this FieldInfo field,
            object target, XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(field, nameof(field));
            Ensure.Argument.NotNull(element, nameof(element));
            ArgOrDefault(ref settings);

            return SetValueImpl(field.SetValue, target, field.FieldType, element, settings);
        }

        public static bool SetValue(this PropertyInfo property,
            object target, XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(property, nameof(property));
            Ensure.Argument.NotNull(element, nameof(element));
            ArgOrDefault(ref settings);

            return SetValueImpl(property.SetValue, target, property.PropertyType, element, settings);
        }

        private static bool SetValueImpl(Action<object, object> valueSetter,
            object target, Type memberType, XElement element, XConvertSettings settings)
        {
            var actualType = ParseObjectType(element);
            if (actualType == null)
                return false;
            if (!memberType.IsAssignableFrom(actualType))
                return false;

            var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, actualType);
            valueSetter(target, converter.ToObject(element, settings));
            return true;
        }

        #endregion XElement To Object

        #endregion Convert

        #region Utilities

        /// <summary>
        ///     Name of the attribute that sepcifies the type of an object.
        /// </summary>
        public const string TypeAttributeName = "Type";

        /// <summary>
        ///     Attribute name for <see langword="null" /> type.
        /// </summary>
        public const string NullTypeName = "$null";

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

        public static string XElementNameOf(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<XConvertMemberAttribute>();
            return attr != null ? attr.Name : member.Name;
        }

        public static bool IsValidXElementName(string name)
        {
            try
            {
                XmlConvert.VerifyName(name);
            }
            catch (XmlException)
            {
                return false;
            }
            return true;
        }

        internal static void ArgOrDefault(ref XConvertSettings settings)
        {
            if (settings == null)
                settings = XConvertSettings.Default;
        }

        #endregion Utilities
    }
}