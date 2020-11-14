using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IReadOnlyList<AssignableMember> CollectConvertMembers(Type type)
        {
            return type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(CanConvert) // All valid members.
                .Select(AssignableMember.Get) // To assignment members.
                .Where(m => m.CanSetValue && m.CanGetValue)
                .GroupBy(m => m.Name.ToLower()) // Group by member name ignore case. (Merge similar members)
                .Select(g => g.OrderByDescending(m => m, XConvertMemberPriorityComparer.Instance).First())
                .ToList();
        }

        #region Object To XElement

        public static XElement ToXElement(this object @object, string elementName, XConvertSettings settings = null)
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

        public static XElement ToXElement(this MemberInfo member, object target, XConvertSettings settings = null)
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

        public static XElement ToXElement(this FieldInfo field, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(field, nameof(field));
            if (!field.IsStatic)
                Ensure.Argument.NotNull(target, nameof(target));
            ArgOrDefault(ref settings);

            var value = field.GetValue(target);
            return ToXElementImpl(value, field.FieldType, XElementNameOf(field), settings);
        }

        public static XElement ToXElement(this PropertyInfo property, object target, XConvertSettings settings = null)
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

        public static XElement ToXElement(this AssignableMember member, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(member, nameof(member));
            ArgOrDefault(ref settings);

            var value = member.GetValue(target);
            return ToXElementImpl(value, member.ValueType, XElementNameOf(member.Info), settings);
        }

        private static XElement ToXElementImpl(
            object @object, Type fallbackType, string elementName, XConvertSettings settings)
        {
            var converter = settings.SelectConverter(@object, fallbackType);
            return converter.ToXElement(@object, elementName, settings);
        }

        /// <summary>
        ///     Get or add the child xml element corresponding to the assignment member specified by arguments.
        /// </summary>
        public static void SetChild(this XElement element,
            Type type, string memberName, object target, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.NotNullOrEmpty(memberName, nameof(memberName));

            var am = AssignableMember.Get(type, memberName);
            if (!am.CanGetValue)
                throw new MissingMemberException($"Unable to get value from {type}.{memberName}.");

            var childName = XElementNameOf(am.Info);
            element.Element(childName)?.Remove();

            var value = am.GetValue(target);
            var child = value.ToXElement(childName, settings);
            element.Add(child);
        }

        public static void SetChildren(this XElement element,
            IEnumerable<string> memberNames, object target, XConvertSettings settings)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            Ensure.Argument.NotNull(memberNames, nameof(memberNames));
            Ensure.Argument.NotNull(target, nameof(target));

            var type = target.GetType();
            foreach (var name in memberNames)
                element.SetChild(type, name, target, settings);
        }

        #endregion Object To XElement

        #region XElement To Object

        public static object ToObject(this XElement element, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            ArgOrDefault(ref settings);

            var type = ParseObjectType(element);
            if (type == null)
                return null;

            var converter = settings.SelectConverter(type);
            return converter.ToObject(element, settings);
        }

        public static bool SetValue(this MemberInfo member,
            object target, XElement valueElement, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(member, nameof(member));

            switch (member)
            {
                case FieldInfo field:
                    return SetValue(field, target, valueElement, settings);
                case PropertyInfo property:
                    return SetValue(property, target, valueElement, settings);
                default:
                    throw new ArgumentException($"Attempt to set value of {member.MemberType}.", nameof(member));
            }
        }

        public static bool SetValue(this FieldInfo field,
            object target, XElement valueElement, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(field, nameof(field));
            Ensure.Argument.NotNull(valueElement, nameof(valueElement));
            ArgOrDefault(ref settings);

            return SetValueImpl(field.SetValue, target, field.FieldType, valueElement, settings);
        }

        public static bool SetValue(this PropertyInfo property,
            object target, XElement valueElement, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(property, nameof(property));
            Ensure.Argument.NotNull(valueElement, nameof(valueElement));
            ArgOrDefault(ref settings);

            if (property.SetMethod == null)
                return false;

            return SetValueImpl(property.SetValue, target, property.PropertyType, valueElement, settings);
        }

        public static bool SetValue(this AssignableMember member,
            object target, XElement valueElement, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(member, nameof(member));
            Ensure.Argument.NotNull(valueElement, nameof(valueElement));
            ArgOrDefault(ref settings);

            if (!member.CanSetValue)
                return false;

            return SetValueImpl(member.SetValue, target, member.ValueType, valueElement, settings);
        }

        private static bool SetValueImpl(Action<object, object> valueSetter,
            object target, Type memberType, XElement element, XConvertSettings settings)
        {
            var actualType = ParseObjectType(element);
            if (actualType == null)
                return false;
            if (!memberType.IsAssignableFrom(actualType))
                return false;

            var converter = settings.SelectConverter(actualType);
            valueSetter(target, converter.ToObject(element, settings));
            return true;
        }

        public static IEnumerable<KeyValuePair<string, bool>> SetMemberValues(this object obj,
            XElement element, IEnumerable<string> memberNames, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(obj, nameof(obj));
            Ensure.Argument.NotNull(memberNames, nameof(memberNames));

            var type = obj.GetType();
            return from name in memberNames
                let am = AssignableMember.Get(type, name)
                let childElement = element.Element(XElementNameOf(am.Info))
                let set = am.SetValue(obj, childElement, settings)
                select new KeyValuePair<string, bool>(name, set);
        }

        #endregion XElement To Object

        #endregion Convert

        #region Utilities

        #region Attribute Read/Write

        /// <summary>
        ///     Attribute value for <see langword="null" /> value.
        /// </summary>
        public const string NullValueName = "$null";

        private static void WriteAttributeValue(XElement element, string attributeName, string value)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                element.Add(new XAttribute(attributeName, value));
            else
                attr.Value = value;
        }

        private static T ParseAttributeValue<T>(XElement element, string attributeName, Func<string, T> parser)
        {
            return parser(GetAttributeValue(element, attributeName));
        }

        private static bool TryParseAttributeValue<T>(XElement element,
            string attributeName, Func<string, T> parser, out T value)
        {
            try
            {
                value = ParseAttributeValue(element, attributeName, parser);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static string GetAttributeValue(XElement element, string attributeName)
        {
            var attr = element.Attribute(attributeName);
            if (attr == null)
                throw new XmlException($"Missing attribute '{attributeName}'.");
            return attr.Value;
        }

        private static bool TryGetAttributeValue(XElement element, string attributeName, out string attributeValue)
        {
            attributeValue = default;
            var attr = element.Attribute(attributeName);
            if (attr == null)
                return false;
            attributeValue = attr.Value;
            return true;
        }

        #region Type Attribute

        /// <summary>
        ///     Name of the attribute that sepcifies the type of an object.
        /// </summary>
        public const string TypeAttributeName = "Type";

        public static void WriteObjectType(XElement element, Type type,
            FormatterAssemblyStyle? style = FormatterAssemblyStyle.Simple)
        {
            WriteAttributeValue(element, TypeAttributeName,
                type == null ? NullValueName : TypeName.Get(type).ToString(style));
        }

        public static Type ParseObjectType(XElement element)
        {
            return ParseAttributeValue(element, TypeAttributeName, ParseTypeAttributeValue);
        }

        private static Type ParseTypeAttributeValue(string value)
        {
            if (value == NullValueName)
                return null;

            var type = TypeCache.GetType(value);
            if (type == null)
                throw new XmlException($@"Failed to load type ""{value}"".");

            return type;
        }

        #endregion Type Attribute

        #region ID Attribute

        public const string IDAttributeName = "ID";

        public static void WriteObjectID(XElement element, long id)
        {
            WriteAttributeValue(element, IDAttributeName, id.ToString());
        }

        public static long ParseObjectID(XElement element)
        {
            return ParseAttributeValue(element, IDAttributeName, long.Parse);
        }

        public static bool TryParseObjectID(XElement element, out long id)
        {
            return TryParseAttributeValue(element, IDAttributeName, long.Parse, out id);
        }

        #endregion ID Attribute

        #region Null Flag Attribute

        public const string NullFlagAttributeName = "IsNull";

        public static void WriteNullFlag(XElement element, bool isNull)
        {
            WriteAttributeValue(element, NullFlagAttributeName, isNull.ToString());
        }

        public static bool ParseNullFlag(XElement element)
        {
            return TryGetAttributeValue(element, NullFlagAttributeName, out var value) && bool.Parse(value);
        }

        public static bool TryParseNullFlag(XElement element, out bool isNull)
        {
            isNull = default;
            return TryGetAttributeValue(element, NullFlagAttributeName, out var value)
                && bool.TryParse(value, out isNull);
        }

        #endregion Null Flag Attribute

        #region Reference Attribute

        public const string ReferenceTypeAttributeValue = "$ref";

        public static void WriteObjectReference(XElement element, long objectID)
        {
            WriteAttributeValue(element, TypeAttributeName, ReferenceTypeAttributeValue);
            WriteObjectID(element, objectID);
        }

        public static long ParseObjectReference(XElement element)
        {
            var typeAttrValue = GetAttributeValue(element, TypeAttributeName);
            if (typeAttrValue != ReferenceTypeAttributeValue)
            {
                throw new XmlException(
                    $"Attribute value '{ReferenceTypeAttributeValue}' for '{TypeAttributeName}' "
                    + "is required for object reference XElement.");
            }
            return ParseObjectID(element);
        }

        public static bool TryParseObjectReference(XElement element, out long objectID)
        {
            objectID = default;
            if (!TryGetAttributeValue(element, TypeAttributeName, out var typeAttrValue))
                return false;
            if (typeAttrValue != ReferenceTypeAttributeValue)
                return false;
            return TryParseObjectID(element, out objectID);
        }

        #endregion Reference Attribute

        #endregion Attribute Read/Write

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