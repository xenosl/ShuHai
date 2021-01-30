using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.Reflection
{
    /// <summary>
    ///     Encapsulates unified member access mechanism for properties and fields through reflection.
    /// </summary>
    public class AssignableMember
    {
        public MemberInfo Info { get; }

        public Type ValueType { get; }

        public string Name => Info.Name;

        public bool IsStatic => Info.IsStatic();

        public bool CanSetValue
        {
            get
            {
                switch (Info)
                {
                    case FieldInfo _:
                        return true;
                    case PropertyInfo prop:
                        return prop.SetMethod != null;
                    default:
                        throw new InvalidReferenceException();
                }
            }
        }

        public bool CanGetValue
        {
            get
            {
                switch (Info)
                {
                    case FieldInfo _:
                        return true;
                    case PropertyInfo prop:
                        return prop.GetMethod != null;
                    default:
                        throw new InvalidReferenceException();
                }
            }
        }

        public void SetValue(object target, object value)
        {
            switch (Info)
            {
                case FieldInfo field:
                    field.SetValue(target, value);
                    break;
                case PropertyInfo prop:
                    prop.SetValue(target, value);
                    break;
                default:
                    throw new InvalidReferenceException();
            }
        }

        public object GetValue(object target)
        {
            switch (Info)
            {
                case FieldInfo field:
                    return field.GetValue(target);
                case PropertyInfo prop:
                    return prop.GetValue(target);
                default:
                    throw new InvalidReferenceException();
            }
        }

        protected AssignableMember(MemberInfo info)
        {
            Info = info;
            ValueType = ValueTypeOf(Info);
        }

        public static MemberInfo GetInfo(Type type, string memberName,
            BindingFlags bindingAttr = BindingAttributes.All)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.NotNullOrEmpty(memberName, nameof(memberName));

            var prop = type.GetProperty(memberName, bindingAttr);
            if (prop != null)
                return prop;
            var field = type.GetField(memberName, bindingAttr);
            if (field != null)
                return field;
            return null;
        }

        public static Type ValueTypeOf(MemberInfo info)
        {
            Ensure.Argument.NotNull(info, nameof(info));

            switch (info)
            {
                case FieldInfo field:
                    return field.FieldType;
                case PropertyInfo prop:
                    return prop.PropertyType;
                default:
                    throw new ArgumentException("Field or property expected.", nameof(info));
            }
        }

        public static bool IsValidMemberType(MemberTypes type)
        {
            switch (type)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                    return true;
                default:
                    return false;
            }
        }

        #region Instances

        public static bool TryGet(Type type, string memberName, out AssignableMember member)
        {
            return TryGet(type, memberName, BindingAttributes.All, out member);
        }

        public static bool TryGet(Type type, string memberName, BindingFlags bindingAttr, out AssignableMember member)
        {
            var (m, e) = GetImpl(type, memberName, bindingAttr);
            member = m;
            return e == null;
        }

        public static bool TryGet(MemberInfo info, out AssignableMember member)
        {
            Ensure.Argument.NotNull(info, nameof(info));

            var (m, e) = GetImpl(info);
            member = m;
            return e == null;
        }

        public static AssignableMember Get(Type type, string memberName,
            BindingFlags bindingAttr = BindingAttributes.All)
        {
            var (m, e) = GetImpl(type, memberName, bindingAttr);
            if (e != null)
                throw e;
            return m;
        }

        public static AssignableMember Get(MemberInfo info)
        {
            Ensure.Argument.NotNull(info, nameof(info));

            var (m, e) = GetImpl(info);
            if (e != null)
                throw e;
            return m;
        }

        private static (AssignableMember member, Exception exception) GetImpl(
            Type type, string memberName, BindingFlags bindingAttr)
        {
            var info = GetInfo(type, memberName, bindingAttr);
            return info == null
                ? (null, new ArgumentException("Member with specified declare type and name not found."))
                : GetImpl(info);
        }

        private static (AssignableMember member, Exception exception) GetImpl(MemberInfo info)
        {
            if (!IsValidMemberType(info.MemberType))
            {
                var e = new ArgumentException(
                    "Only field or property is considered as assignable member.", nameof(info));
                return (null, e);
            }

            if (!_instances.TryGetValue(info, out var member))
            {
                member = new AssignableMember(info);
                _instances.Add(info, member);
            }
            return (member, null);
        }

        private static readonly Dictionary<MemberInfo, AssignableMember>
            _instances = new Dictionary<MemberInfo, AssignableMember>();

        #endregion Instances
    }
}