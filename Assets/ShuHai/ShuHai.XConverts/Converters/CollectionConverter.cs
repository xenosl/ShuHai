using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(ICollection<>))]
    public class CollectionConverter : XConverter
    {
        public new static CollectionConverter Default { get; } = new CollectionConverter();

        #region Object To XElement

        protected override void PopulateXElementAttributes(XElement element,
            object @object, XConvertSettings settings, XConvertToXElementSession session)
        {
            base.PopulateXElementAttributes(element, @object, settings, session);

            if (@object.GetType().IsArray)
                WriteArrayLengths(element, (Array)@object);
        }


        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var itemType = ItemTypeOf(@object);
            var collection = (IEnumerable)@object;
            foreach (var item in collection)
            {
                var converter = settings.SelectConverter(item, itemType);
                var childElement = converter.ToXElement(item, "Item", settings);
                element.Add(childElement);
            }
        }

        #endregion Object To XElement

        #region XElement To Object

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return type.IsArray
                ? Array.CreateInstance(type.GetElementType(), ParseArrayLengths(element))
                : base.CreateObject(element, type, settings);
        }

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            var type = @object.GetType();
            if (type.IsArray)
            {
                var array = (Array)@object;
                int index = 0;
                foreach (var item in ChildrenToObjects(element, settings))
                    array.SetValue(item, index++);
            }
            else
            {
                var collectionType = CollectionTypeOf(@object);
                var addMethod = collectionType.GetMethod("Add", new[] { ItemTypeOf(collectionType) });
                foreach (var item in ChildrenToObjects(element, settings))
                    addMethod.Invoke(@object, new[] { item });
            }
        }

        private static IEnumerable<object> ChildrenToObjects(XElement element, XConvertSettings settings)
        {
            return element.Elements().Select(e => e.ToObject(settings));
        }

        #endregion XElement To Object

        #region Utilities

        /// <summary>
        ///     Get the actual item type of the specified collection object.
        /// </summary>
        protected Type ItemTypeOf(object @object) { return ItemTypeOf(CollectionTypeOf(@object)); }

        protected Type ItemTypeOf(Type collectionType)
        {
            Debug.Assert(collectionType.IsClosedConstructedTypeOf(ConvertType));
            return collectionType.GetGenericArguments()[0];
        }

        /// <summary>
        ///     Get the constructed collection type of the specified collection object.
        /// </summary>
        protected Type CollectionTypeOf(object @object)
        {
            return @object.GetType().GetInterfaces()
                .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == ConvertType);
        }

        private const string ArrayLengthsAttributeName = "Lengths";

        private static void WriteArrayLengths(XElement element, Array array)
        {
            element.SetAttributeValue(ArrayLengthsAttributeName, string.Join(",", EnumerateArrayLength(array)));
        }

        private static IEnumerable<int> EnumerateArrayLength(Array array)
        {
            for (int i = 0; i < array.Rank; ++i)
                yield return array.GetLength(i);
        }

        private static int[] ParseArrayLengths(XElement element)
        {
            var attr = element.Attribute(ArrayLengthsAttributeName);
            if (attr == null)
                throw new XmlException($"Attribute '{ArrayLengthsAttributeName}' not found.");
            return attr.Value.Split(',').Select(int.Parse).ToArray();
        }

        #endregion Utilities
    }
}