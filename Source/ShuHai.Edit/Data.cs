using System;
using System.Collections.Generic;
using System.Linq;

namespace ShuHai.Edit
{
    public abstract class Data : IEquatable<Data>
    {
        #region Convert

        public static Data ToData(object @object,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            if (ValueData.TryCreate(@object, out var valueData))
                return valueData;
            return ObjectDataConverter.ToData(null, @object, settings, session);
        }

        public static IEnumerable<Data> ToDatas(IEnumerable<object> objects,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            return objects.Select(o => ToData(o, settings, session));
        }

        public static object ToObject(Data data,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            switch (data)
            {
                case null:
                    throw new ArgumentNullException(nameof(data));
                case ValueData valueData:
                    return valueData.Value;
                case ObjectData objectData:
                    return ObjectDataConverter.ToObject(null, objectData, settings, session);
                default:
                    throw new NotSupportedException($"Create object for '{data.GetType()}' is not supported.");
            }
        }

        public static IEnumerable<object> ToObjects(IEnumerable<Data> datas,
            ObjectDataConvertSettings settings = null, ObjectDataConvertSession session = null)
        {
            return datas.Select(d => ToObject(d, settings, session));
        }

        #endregion Convert

        public abstract bool Equals(Data other);
    }
}