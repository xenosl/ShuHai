using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShuHai.Edit
{
    public sealed class ObjectData : Data, IEquatable<ObjectData>
    {
        public Type ObjectType { get; }

        internal ObjectData(Type objectType, IEnumerable<Member> members)
        {
            Debug.Assert(objectType != null);

            ObjectType = objectType;

            Members = new List<Member>(members);
            _membersHashCode = CalculateMemberListHashCode();

            _memberDict = CreateMemberDict();
        }

        #region Members

        public struct Member : IEquatable<Member>
        {
            public string Key { get; }

            public Data Value { get; }

            public Member(string key, Data value)
            {
                Ensure.Argument.NotNullOrEmpty(key, nameof(key));
                Ensure.Argument.NotNull(value, nameof(value));

                Key = key;
                Value = value;
            }

            public Member(KeyValuePair<string, Data> pair) : this(pair.Key, pair.Value) { }

            public T GetUnderlyingValue<T>()
            {
                if (!(Value is ValueData vd))
                    throw new NotSupportedException($"Get underlying value for '{Value.GetType()}' is not supported.");
                return (T)vd.Value;
            }

            public bool Equals(Member other) { return Key.Equals(other.Key) && Equals(Value, other.Value); }

            public override bool Equals(object obj) { return obj is Member other && Equals(other); }

            public override int GetHashCode() { return Hash.Combine(Key.GetHashCode(), Value.GetHashCode()); }

            public static bool operator ==(Member l, Member r) { return l.Equals(r); }
            public static bool operator !=(Member l, Member r) { return !(l == r); }
        }

        public IReadOnlyList<Member> Members { get; }

        private readonly int _membersHashCode;

        private int CalculateMemberListHashCode()
        {
            int h = 0;
            for (int i = 0; i < Members.Count - 1; ++i)
            {
                var m = Members[i];
                h = Hash.Combine(h, m.GetHashCode());
            }
            return h;
        }

        private bool MemberListEquals(IReadOnlyList<Member> other)
        {
            if (Members.Count != other.Count)
                return false;

            for (int i = 0; i < Members.Count; ++i)
            {
                if (Members[i] != other[i])
                    return false;
            }
            return true;
        }

        #region Dict

        public bool TryGetMember(string key, out Member member)
        {
            var (m, e) = GetMemberImpl(key);
            member = m;
            return e != null;
        }

        public bool TryGetMembers(string key, out IReadOnlyCollection<Member> members)
        {
            var (list, e) = GetMembersImpl(key);
            members = list;
            return e != null;
        }

        public Member GetMember(string key)
        {
            var (m, e) = GetMemberImpl(key);
            if (e != null)
                throw e;
            return m;
        }

        public Data GetValue(string key) { return GetMember(key).Value; }

        public T GetUnderlyingValue<T>(string key) { return GetMember(key).GetUnderlyingValue<T>(); }

        public IReadOnlyCollection<Member> GetMembers(string key)
        {
            var (list, e) = GetMembersImpl(key);
            if (e != null)
                throw e;
            return list;
        }

        private readonly IReadOnlyDictionary<string, IReadOnlyCollection<Member>> _memberDict;

        private (Member member, Exception e) GetMemberImpl(string key)
        {
            var (list, e) = GetMembersImpl(key);
            if (e != null || list.Count == 0)
                return (default, e);
            return (list.First(), null);
        }

        private (IReadOnlyCollection<Member> members, Exception e) GetMembersImpl(string key)
        {
            if (!_memberDict.TryGetValue(key, out var list))
                return (Array.Empty<Member>(), new KeyNotFoundException($"Members with key '{key}' not found."));
            return (list, null);
        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<Member>> CreateMemberDict()
        {
            var dict = new Dictionary<string, List<Member>>();
            foreach (var member in Members)
            {
                if (!dict.TryGetValue(member.Key, out var list))
                {
                    list = new List<Member>();
                    dict.Add(member.Key, list);
                }
                list.Add(member);
            }
            return dict.ToDictionary(p => p.Key, p => (IReadOnlyCollection<Member>)p.Value);
        }

        #endregion Dict

        #endregion Members

        #region Equality

        public override bool Equals(Data other) { return Equals(other as ObjectData); }

        public bool Equals(ObjectData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ObjectType == other.ObjectType && MemberListEquals(other.Members);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ObjectData other && Equals(other);
        }

        public override int GetHashCode() { return Hash.Combine(ObjectType.GetHashCode(), _membersHashCode); }

        public static bool operator ==(ObjectData l, ObjectData r) { return l?.Equals(r) ?? ReferenceEquals(r, null); }
        public static bool operator !=(ObjectData l, ObjectData r) { return !(l == r); }

        #endregion Equality
    }
}