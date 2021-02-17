using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShuHai.Edit.ObjectDataConverters
{
    [ObjectDataConvertType(typeof(ICollection<>))]
    public class CollectionConverter : ObjectDataConverter
    {
        public new static CollectionConverter Default { get; } = new CollectionConverter();

        public override bool CanConvert(Type type)
        {
            if (!base.CanConvert(type))
                return false;

            if (type.IsArray && type.GetArrayRank() != 1)
                return false;
            return true;
        }

        protected override IEnumerable<ObjectData.Member> ToDataMembers(
            object @object, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            var dataMembers = new List<ObjectData.Member>();
            //var itemType = ItemTypeOf(@object);
            var collection = (IEnumerable)@object;
            foreach (var item in collection)
            {
                var data = Data.ToData(item, settings, session);
                dataMembers.Add(new ObjectData.Member("Item", data));
            }
            return dataMembers;
        }

        protected override object CreateObject(ObjectData data,
            ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            var type = data.ObjectType;
            return type.IsArray
                ? Array.CreateInstance(type.GetElementType(), data.Members.Count)
                : base.CreateObject(data, settings, session);
        }

        protected override void PopulateObjectMembers(
            object @object, ObjectData data, ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            var type = @object.GetType();
            if (type.IsArray)
            {
                var array = (Array)@object;
                int index = 0;
                foreach (var item in MembersToObjects(data, settings, session))
                    array.SetValue(item, index++);
            }
            else
            {
                var collectionType = CollectionTypeOf(@object);
                var addMethod = collectionType.GetMethod("Add", new[] { ItemTypeOf(collectionType) });
                Debug.Assert(addMethod != null);
                foreach (var item in MembersToObjects(data, settings, session))
                    addMethod.Invoke(@object, new[] { item });
            }
        }

        private static IEnumerable<object> MembersToObjects(ObjectData data,
            ObjectDataConvertSettings settings, ObjectDataConvertSession session)
        {
            return data.Members.Select(m => Data.ToObject(m.Value, settings, session));
        }

        #region Utilities

        /// <summary>
        ///     Get the actual item type of the specified collection object.
        /// </summary>
        private Type ItemTypeOf(object @object) { return ItemTypeOf(CollectionTypeOf(@object)); }

        private Type ItemTypeOf(Type collectionType)
        {
            Debug.Assert(collectionType.IsClosedConstructedTypeOf(ConvertType));
            return collectionType.GetGenericArguments()[0];
        }

        /// <summary>
        ///     Get the constructed collection interface type of the specified collection object.
        /// </summary>
        private Type CollectionTypeOf(object @object)
        {
            return @object.GetType().GetInterfaces()
                .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == ConvertType);
        }

        #endregion Utilities
    }
}