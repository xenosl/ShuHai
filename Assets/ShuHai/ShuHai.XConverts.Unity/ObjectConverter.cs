using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;

namespace ShuHai.XConverts.Unity
{
    using UObject = UnityEngine.Object;

    [XConvertType(typeof(UObject))]
    public abstract class ObjectConverter : XConverter
    {
        private static class MemberNames
        {
            public const string Name = "name";
        }

        #region Object To XElement

        protected override void PopulateXElement(XElement element,
            object @object, XConvertSettings settings, XConvertToXElementSession session)
        {
            element.RemoveAll();
            PopulateXElementAttributes(element, @object, settings, session);

            if ((UObject)@object)
            {
                PopulateXElementValue(element, @object, settings);
                PopulateXElementChildren(element, @object, settings);
            }
        }

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var obj = (UObject)@object;
            element.Add(StringConverter.Default.ToXElement(obj.name, MemberNames.Name, settings));
        }

        #endregion Object To XElement

        #region XElement To Object

        public new UObject ToObject(XElement element,
            XConvertSettings settings = null, XConvertToObjectSession session = null)
        {
            return (UObject)base.ToObject(element, settings, session);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            throw new NotImplementedException($"Create object for '{GetType()}' is not implemented yet.");
        }

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var obj = (UObject)@object;
            obj.name = StringConverter.Default.ToString(element.Element(MemberNames.Name), settings);
        }

        #endregion XElement To Object
    }
}