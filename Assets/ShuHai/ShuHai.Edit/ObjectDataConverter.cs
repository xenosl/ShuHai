using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ShuHai.Reflection;

namespace ShuHai.Edit
{
    [ObjectDataConvertType(typeof(object))]
    public class ObjectDataConverter
    {
        public static ObjectDataConverter Default { get; } = new ObjectDataConverter();

        protected ObjectDataConverter() { ConvertType = GetConvertType(GetType()); }

        #region Convert Type

        public Type ConvertType { get; }

        public virtual bool CanConvert(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));

            if (!type.IsInstantiable())
                return false;

            return ConvertType.IsGenericType
                ? type.IsAssignableToConstructedGenericType(ConvertType)
                : ConvertType.IsAssignableFrom(type);
        }

        public static Type GetConvertType(Type converterType)
        {
            Ensure.Argument.NotNull(converterType, nameof(converterType));
            Ensure.Argument.Is<ObjectDataConverter>(converterType, nameof(converterType));

            var type = converterType;
            while (type != typeof(object))
            {
                var attr = converterType.GetCustomAttribute<ObjectDataConvertTypeAttribute>();
                if (attr != null)
                    return attr.ConvertType;
                type = type.BaseType;
            }

            throw new MissingAttributeException(typeof(ObjectDataConvertTypeAttribute));
        }

        #endregion Convert Type

        #region Object To Data

        public ObjectData ToData(object @object,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            if (ValueData.CanCreate(@object))
                throw new ArgumentException($"Object can be only converted to {typeof(ValueData)}", nameof(@object));

            return ToData(this, @object, settings, session);
        }

        protected virtual IEnumerable<ObjectData.Member> ToDataMembers(
            object @object, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            var type = @object.GetType();
            return SelectDataMembers(type).Select(am => new ObjectData.Member(
                am.Name, Data.ToData(am.GetValue(@object), settings, session)));
        }

        private static IEnumerable<AssignableMember> SelectDataMembers(Type type)
        {
            return type.GetMembers()
                .Where(m => !m.IsDefined(typeof(CompilerGeneratedAttribute)))
                .Select(m =>
                {
                    var got = AssignableMember.TryGet(m, out var am);
                    return (got, am);
                })
                .Where(t => t.got && t.am.CanGetValue)
                .Select(t => t.am);
        }

        internal static ObjectData ToData(ObjectDataConverter converter,
            object @object, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            if (session == null)
                session = new ObjectDataConvertSession();
            if (session.TryGetConvertedData(@object, out var data))
                return data;

            if (settings == null)
                settings = ObjectDataConvertSettings.Default;

            var type = @object.GetType();
            if (converter == null)
                converter = SelectConverter(settings, type);

            var members = converter.ToDataMembers(@object, settings, session);
            data = new ObjectData(type, members);
            session.AddConverted(@object, data);
            return data;
        }

        #endregion Object To Data

        #region Data To Object

        public object ToObject(ObjectData data,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            Ensure.Argument.NotNull(data, nameof(data));

            return ToObject(this, data, settings, session);
        }

        protected virtual object CreateObject(ObjectData data,
            ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            return Activator.CreateInstance(data.ObjectType,
                BindingAttributes.DeclareInstance, null, Array.Empty<object>(), CultureInfo.InvariantCulture);
        }

        protected virtual void PopulateObjectMembers(object @object,
            ObjectData data, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            var type = @object.GetType();
            foreach (var member in data.Members)
            {
                var am = AssignableMember.Get(type, member.Key, BindingAttributes.Instance);
                var value = Data.ToObject(member.Value, settings, session);
                am.SetValue(@object, value);
            }
        }

        private static object CreateObjectByDataConstructor(ObjectData data)
        {
            var ctor = data.ObjectType.GetConstructor(
                BindingAttributes.DeclareInstance, null, new[] { typeof(ObjectData) }, null);
            return ctor != null ? ctor.Invoke(new object[] { data }) : null;
        }

        internal static object ToObject(ObjectDataConverter converter,
            ObjectData data, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            if (session == null)
                session = new ObjectDataConvertSession();
            if (session.TryGetConvertedObject(data, out var @object))
                return @object;

            if (settings == null)
                settings = ObjectDataConvertSettings.Default;

            if (converter == null)
                converter = SelectConverter(settings, data.ObjectType);

            @object = CreateObjectByDataConstructor(data);
            if (!ReferenceEquals(@object, null))
            {
                session.AddConverted(@object, data);
                return @object;
            }

            @object = converter.CreateObject(data, settings, session);
            if (ReferenceEquals(@object, null))
                return null;

            converter.PopulateObjectMembers(@object, data, settings, session);
            session.AddConverted(@object, data);
            return @object;
        }

        #endregion Data To Object

        #region Utilities

        private static ObjectDataConverter SelectConverter(ObjectDataConvertSettings settings, Type convertType)
        {
            if (settings == null)
                settings = ObjectDataConvertSettings.Default;
            return SelectConverter(settings.Selector, settings.Converters, convertType);
        }

        private static ObjectDataConverter SelectConverter(
            ObjectDataConverterSelector selector, ObjectDataConverterCollection collection, Type convertType)
        {
            if (selector == null)
                selector = ObjectDataConverterSelector.Default;
            var converter = selector.Select(collection ?? ObjectDataConverterCollection.Default, convertType);
            return converter ?? Default;
        }

        #endregion Utilities
    }

    internal static class MemberKeyParser
    {
        public static string ToString(AssignableMember am, Type objectType)
        {
            return ToString(am.Info.DeclaringType, objectType, am.Name);
        }

        public static string ToString(Type declaringType, Type actualType, string memberName)
        {
            return ToString(actualType.GetDeriveDepth(declaringType), memberName);
        }

        public static string ToString(int declareDepth, string memberName)
        {
            return declareDepth == 0 ? memberName : declareDepth + "." + memberName;
        }

        public static (int declareDepth, string name) FromString(string str)
        {
            Ensure.Argument.NotNull(str, nameof(str));

            try
            {
                var l = str.Split('.');
                switch (l.Length)
                {
                    case 1:
                        return (0, l[0]);
                    case 2:
                        return (int.Parse(l[0]), l[1]);
                    default:
                        throw new ArgumentException("Invalid key string", nameof(str));
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to parse member key from string '{str}'.", nameof(str), e);
            }
        }
    }
}