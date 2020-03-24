using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ShuHai.XConverts;

namespace ShuHai.EditSystem
{
    using ObjectCreator = Func<object[], object>;

    public class Editor
    {
        #region Objects

        public event Action<EditorObject> ObjectAdded;
        public event Action<EditorObject> ObjectRemoving;

        public ICollection<EditorObject> Objects => _objects.Values;

        public EditorObject AddObject(object rawObject)
        {
            var obj = new EditorObject(_nextObjectOrder++, rawObject);
            AddObject(obj);
            return obj;
        }

        public bool RemoveObject(EditorObject obj)
        {
            Ensure.Argument.NotNull(obj, nameof(obj));
            return RemoveObject(obj.Order);
        }

        public EditorObject GetObject(int order) { return _objects.TryGetValue(order, out var obj) ? obj : null; }

        private readonly Dictionary<int, EditorObject> _objects = new Dictionary<int, EditorObject>();

        private void AddObject(EditorObject obj)
        {
            _objects.Add(obj.Order, obj);
            obj.Owner = this;
            
            ObjectAdded?.Invoke(obj);
        }

        private bool RemoveObject(int order)
        {
            if (!_objects.TryGetValue(order, out var obj))
                return false;

            ObjectRemoving?.Invoke(obj);
            
            obj.Owner = null;
            _objects.Remove(order);
            
            return true;
        }

        private void ClearObjects()
        {
            var ids = _objects.Keys.ToArray();
            foreach (var id in ids)
                RemoveObject(id);
        }

        private int _nextObjectOrder = 1;

        #region Serialization

        public XElement Serialize(string elementName)
        {
            var root = new XElement(elementName);
            var objects = _objects.Values.OrderBy(w => w.Order);
            foreach (var eo in objects)
                root.Add(XConvert.ToXElement(eo.Value, eo.GetType().Name));
            return root;
        }

        public void Deserialize(XElement root)
        {
            ClearObjects();
            foreach (var data in root.Elements())
                AddObject(XConvert.ToObject(data));
        }

        #endregion Serialization

        #endregion Objects

        #region Undo/Redo

        public void Undo() { throw new NotImplementedException(); }

        public void Redo() { throw new NotImplementedException(); }

        #endregion Undo/Redo
    }
}