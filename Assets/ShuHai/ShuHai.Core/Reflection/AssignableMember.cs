using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.Reflection
{
    public sealed class AssignableMember
    {
        public MemberInfo Member { get; }

        public void SetValue(object target, object value)
        {
            switch (Member)
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
            switch (Member)
            {
                case FieldInfo field:
                    return field.GetValue(target);
                case PropertyInfo prop:
                    return prop.GetValue(target);
                default:
                    throw new InvalidReferenceException();
            }
        }

        private AssignableMember(MemberInfo member) { Member = member; }

        #region Instances

        public static AssignableMember Get(Type type, string memberName,
            BindingFlags bindingAttr = BindingAttributes.DeclareAll)
        {
            var prop = type.GetProperty(memberName, bindingAttr);
            if (prop != null)
                return Get(prop);
            var field = type.GetField(memberName, bindingAttr);
            if (field != null)
                return Get(field);
            return null;
        }

        public static AssignableMember Get(MemberInfo member)
        {
            Ensure.Argument.NotNull(member, nameof(member));

            var mt = member.MemberType;
            if (mt != MemberTypes.Field && mt != MemberTypes.Property)
            {
                throw new ArgumentException(
                    "Only field or property is considered as assignable member.", nameof(member));
            }

            if (!_instances.TryGetValue(member, out var am))
            {
                am = new AssignableMember(member);
                _instances.Add(member, am);
            }
            return am;
        }

        private static readonly Dictionary<MemberInfo, AssignableMember>
            _instances = new Dictionary<MemberInfo, AssignableMember>();

        #endregion Instances
    }
}