using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using ShuHai.Reflection;

namespace ShuHai.XConverts
{
    public class XConvertMember : AssignableMember
    {
        public string ElementName { get; }

        public XElement ToElement(object target, XConvertSettings settings = null)
        {
            var value = GetValue(target);
            return XConvert.ToXElement(value, ElementName, settings);
        }

        public void SetValue(object target, XElement element, XConvertSettings settings = null)
        {
            Info.SetValue(target, element, settings);
        }

        protected XConvertMember(MemberInfo info, string elementName) : base(info) { ElementName = elementName; }

        #region Instances

        public static AssignableMember Get(Type type, string memberName,
            BindingFlags bindingAttr = BindingAttributes.DeclareAll, bool throwOnError = true)
        {
            return Get(GetInfo(type, memberName, bindingAttr), throwOnError);
        }

        public static XConvertMember Get(MemberInfo info, bool throwOnError = true)
        {
            if (!_instances.TryGetValue(info, out var xm))
            {
                xm = Create(info, throwOnError);
                if (xm != null)
                    _instances.Add(info, xm);
            }
            return xm;
        }

        private static readonly Dictionary<MemberInfo, XConvertMember>
            _instances = new Dictionary<MemberInfo, XConvertMember>();

        private static XConvertMember Create(MemberInfo info, bool throwOnError)
        {
            if (!XConvert.CanConvert(info))
            {
                if (throwOnError)
                    throw new ArgumentException($"'{info}' is not a XConvert member.");
                return null;
            }

            var elementName = XConvert.XElementNameOf(info);
            var xm = new XConvertMember(info, elementName);
            return xm;
        }

        #endregion Instances
    }
}