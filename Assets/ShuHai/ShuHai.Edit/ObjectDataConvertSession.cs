using System.Collections.Generic;

namespace ShuHai.Edit
{
    public sealed class ObjectDataConvertSession
    {
        public bool AddConverted(object @object, ObjectData data)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));

            if (ValueData.CanCreate(@object))
                return false;

            return _objectToData.TryAdd(@object, data)
                || _dataToObject.TryAdd(data, @object);
        }

        public bool TryGetConvertedData(object @object, out ObjectData data)
        {
            Ensure.Argument.NotNull(@object, nameof(@object));

            return _objectToData.TryGetValue(@object, out data);
        }

        public bool TryGetConvertedObject(ObjectData data, out object @object)
        {
            Ensure.Argument.NotNull(data, nameof(data));

            return _dataToObject.TryGetValue(data, out @object);
        }

        private readonly Dictionary<object, ObjectData> _objectToData = new Dictionary<object, ObjectData>();
        private readonly Dictionary<ObjectData, object> _dataToObject = new Dictionary<ObjectData, object>();
    }
}