using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShuHai
{
    internal static class InternalUtil
    {
        #region Members By Attribute

        public static IReadOnlyDictionary<Type, IReadOnlyCollection<T>> ReadOnlyMembersByAttributeType<T>(
            this IEnumerable<T> members)
            where T : MemberInfo
        {
            return ReadOnlyMembersByAttributeType(members, (m, t) => m);
        }

        public static IReadOnlyDictionary<Type, IReadOnlyCollection<T>> ReadOnlyMembersByAttributeType<T>(
            this IEnumerable<T> members, Func<T, Type, T> resultSelector)
            where T : MemberInfo
        {
            var dict = MembersByAttributeType(members, resultSelector);
            return dict.ToDictionary(p => p.Key, p => (IReadOnlyCollection<T>)p.Value);
        }

        public static Dictionary<Type, List<T>> MembersByAttributeType<T>(
            this IEnumerable<T> members, Func<T, Type, T> resultSelector)
            where T : MemberInfo
        {
            return MembersByAttributeType<Attribute, T>(members, null, resultSelector);
        }

        public static Dictionary<Type, List<TMember>> MembersByAttributeType<TAttribute, TMember>(
            this IEnumerable<TMember> members,
            Func<TAttribute, TMember, bool> resultFilter, Func<TMember, Type, TMember> resultSelector)
            where TAttribute : Attribute
            where TMember : MemberInfo
        {
            var dict = new Dictionary<Type, List<TMember>>();
            foreach (var member in members)
            {
                foreach (var attribute in member.GetCustomAttributes<TAttribute>())
                {
                    if (resultFilter != null && !resultFilter(attribute, member))
                        continue;
                    var attributeType = attribute.GetType();
                    dict.Add(attributeType, resultSelector(member, attributeType));
                }
            }
            return dict;
        }

        #endregion Members By Attribute
    }
}
