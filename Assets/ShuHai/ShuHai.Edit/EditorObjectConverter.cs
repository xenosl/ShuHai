using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ShuHai.Reflection;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    [XConvertType(typeof(EditorObject))]
    public class EditorObjectConverter : XConverter
    {
        private sealed class MemberNames : IEnumerable<string>
        {
            public static readonly MemberNames Instance = new MemberNames();

            public const string Name = "Name";
            public const string Value = "Value";

            public IEnumerator<string> GetEnumerator()
            {
                yield return Name;
                yield return Value;
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        #region Object To XElement

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            foreach (var name in MemberNames.Instance)
                element.SetChild(type, name, @object, settings);
        }

        #endregion Object To XElement

        #region XElement To Object

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var value = element.Element(MemberNames.Value).ToObject(settings);
            return Activator.CreateInstance(type,
                BindingAttributes.DeclareInstance, null, new[] { value }, null);
        }

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var type = @object.GetType();
            foreach (var name in MemberNames.Instance.Where(n => n != MemberNames.Value))
            {
                var am = AssignableMember.Get(type, name);
                var childElement = element.Element(XConvert.XElementNameOf(am.Info));
                am.SetValue(@object, childElement, settings);
            }
        }

        #endregion XElement To Object
    }
}