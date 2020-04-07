using System;
using System.Collections;
using System.Collections.Generic;
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

            public const string Order = "Order";
            public const string Value = "Value";

            public IEnumerator<string> GetEnumerator()
            {
                yield return Order;
                yield return Value;
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        #region Object To XElement

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            foreach (var name in MemberNames.Instance)
                element.Add(AssignableMember.Get(type, name).ToXElement(@object, settings));
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
            base.PopulateObjectMembers(@object, element, settings);
        }

        #endregion XElement To Object
    }
}