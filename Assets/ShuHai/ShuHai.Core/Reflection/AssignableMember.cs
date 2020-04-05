using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.Reflection
{
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
            BindingFlags bindingAttr = BindingAttributes.DeclareAll)
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

        #region Instances

        public static AssignableMember Get(Type type, string memberName,
            BindingFlags bindingAttr = BindingAttributes.DeclareAll)
        {
            return Get(GetInfo(type, memberName, bindingAttr));
        }

        public static AssignableMember Get(MemberInfo info)
        {
            Ensure.Argument.NotNull(info, nameof(info));

            var mt = info.MemberType;
            if (mt != MemberTypes.Field && mt != MemberTypes.Property)
            {
                throw new ArgumentException(
                    "Only field or property is considered as assignable member.", nameof(info));
            }

            if (!_instances.TryGetValue(info, out var am))
            {
                am = new AssignableMember(info);
                _instances.Add(info, am);
            }
            return am;
        }

        private static readonly Dictionary<MemberInfo, AssignableMember>
            _instances = new Dictionary<MemberInfo, AssignableMember>();

        #endregion Instances
    }
}