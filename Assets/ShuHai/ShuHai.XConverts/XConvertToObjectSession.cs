using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ShuHai.XConverts
{
    public class XConvertToObjectSession
    {
        public bool AddObject(XElement element, object @object)
        {
            Ensure.Argument.NotNull(element, nameof(element));
            Ensure.Argument.NotNull(@object, nameof(@object));

            if (XConvert.TryParseObjectID(element, out var id))
            {
                if (_idToObjects.TryGetValue(id, out var objByID))
                {
                    if (!ReferenceEquals(objByID, @object))
                        throw new InvalidOperationException($"Attempt to bind different object with same id({id}).");
                    return false;
                }
                _idToObjects.Add(id, @object);
            }

            if (_elementToObject.TryGetValue(element, out var objByElem))
            {
                if (!ReferenceEquals(objByElem, @object))
                    throw new InvalidOperationException("Attempt to bind different object with same XElement.");
                return false;
            }

            _elementToObject.Add(element, @object);
            return true;
        }

        public object GetObject(long id) { return _idToObjects.TryGetValue(id, out var obj) ? obj : null; }

        public object GetObject(XElement element)
        {
            return _elementToObject.TryGetValue(element, out var @object) ? @object : null;
        }

        private readonly Dictionary<long, object> _idToObjects = new Dictionary<long, object>();
        private readonly Dictionary<XElement, object> _elementToObject = new Dictionary<XElement, object>();
    }
}