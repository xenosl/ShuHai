using System.Xml.Linq;
using ShuHai.Reflection;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    [XConvertType(typeof(Editor))]
    public class EditorConverter : XConverter
    {
        private static class MemberNames
        {
            public const string Name = "Name";
            public const string Objects = "_objects";
            public const string AddObject = "AddObject";
        }

        #region Object To XElement

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var type = @object.GetType();
            var editor = (Editor)@object;
            element.Add(AssignableMember.Get(type, MemberNames.Name).ToXElement(@object, settings));
            element.Add(ObjectsToXElement(editor, settings));
        }

        private XElement ObjectsToXElement(Editor editor, XConvertSettings settings)
        {
            var member = editor.GetType().GetField(MemberNames.Objects, BindingAttributes.DeclareInstance);
            var root = new XElement(XConvert.XElementNameOf(member));
            foreach (var obj in editor.Objects)
            {
                var childElement = obj.ToXElement(typeof(EditorObject).Name, settings);
                childElement.SetAttributeValue("Order", obj.Order);
                root.Add(childElement);
            }
            return root;
        }

        #endregion Object To XElement

        #region XElement To Object

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var editor = (Editor)@object;
            editor.Name = element.Element(MemberNames.Name)?.Value;
            PopulateObjects(editor, element, settings);
        }

        private void PopulateObjects(Editor editor, XElement element, XConvertSettings settings)
        {
            var type = editor.GetType();
            var member = type.GetField(MemberNames.Objects, BindingAttributes.DeclareInstance);
            var addObjectMethod = type.GetMethod(MemberNames.AddObject,
                BindingAttributes.DeclareInstance, new[] { typeof(EditorObject), typeof(int) });

            var root = element.Element(XConvert.XElementNameOf(member));
            foreach (var objectElement in root.Elements())
            {
                var order = int.Parse(objectElement.Attribute("Order").Value);
                var eo = (EditorObject)objectElement.ToObject(settings);
                addObjectMethod.Invoke(editor, new object[] { eo, order });
            }
        }

        #endregion XElement To Object
    }
}