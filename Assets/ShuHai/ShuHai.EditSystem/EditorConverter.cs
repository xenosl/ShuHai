using System.Xml.Linq;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    [XConvertType(typeof(Editor))]
    public class EditorConverter : XConverter
    {
        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var editor = (Editor)@object;
            element.Add(new XElement("Name", editor.Name));
            element.Add(ObjectsToXElement(editor, settings));
        }

        private XElement ObjectsToXElement(Editor editor, XConvertSettings settings)
        {
            var member = editor.GetType().GetField("_objects", BindingAttributes.DeclareInstance);
            var root = new XElement(XConvert.XElementNameOf(member));
            foreach (var obj in editor.Objects)
            {
                var childElement = XConvert.ToXElement(obj, "_" + obj.Order, settings);
                childElement.Add(XConvert.ToXElement(obj.Value, "Value", settings));
                root.Add(childElement);
            }
            return root;
        }

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var editor = (Editor)@object;
            var type = editor.GetType();

            var member = type.GetField("_objects", BindingAttributes.DeclareInstance);
            var root = element.Element(XConvert.XElementNameOf(member));
            PopulateObjects(editor, root, settings);
        }

        private void PopulateObjects(Editor editor, XElement root, XConvertSettings settings)
        {
            var type = editor.GetType();
            var addObjectMethod = type.GetMethod("AddObject", new[] { typeof(EditorObject), typeof(int) });
            foreach (var objectElement in root.Elements())
            {
                var eo = XConvert.ToObject(objectElement, settings);
            }
        }
    }
}