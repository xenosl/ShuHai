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

        [XConvertMember("Objects")]
        private Dictionary<int, EditorObject> _objects = new Dictionary<int, EditorObject>();

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

        [XConvertMember("NextObjectOrder")] private int _nextObjectOrder = 1;

        #endregion Objects

        #region Undo/Redo

        public void Undo() { throw new NotImplementedException(); }

        public void Redo() { throw new NotImplementedException(); }

        #endregion Undo/Redo
    }
}