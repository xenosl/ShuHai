using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(ICollection<>))]
    public class CollectionConverter : XConverter
    {
        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            var itemType = ItemTypeOf(@object);
            var collection = (IEnumerable)@object;
            foreach (var item in collection)
            {
                var converter = settings.FindAppropriateConverter(item, itemType);
                var childElement = converter.ToXElement(item, "Item", settings);
                element.Add(childElement);
            }
        }

        private const BindingFlags BindingAttrForInterfaceMethod =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        protected override void PopulateObjectMembersImpl(object @object, XElement element, XConvertSettings settings)
        {
            var type = CollectionTypeOf(@object);
            var addMethod = type.GetMethod("Add", new[] { ItemTypeOf(type) });
            foreach (var childElement in element.Elements())
            {
                var itemType = ParseObjectType(childElement);
                object item = null;
                if (itemType != null)
                {
                    var converter = settings.FindAppropriateConverter(itemType);
                    item = converter.ToObject(childElement, settings);
                }
                addMethod.Invoke(@object, new[] { item });
            }
        }

        private Type ItemTypeOf(object @object) { return ItemTypeOf(CollectionTypeOf(@object)); }

        private Type ItemTypeOf(Type collectionType)
        {
            Debug.Assert(collectionType.IsClosedConstructedTypeOf(ConvertType));
            return collectionType.GetGenericArguments()[0];
        }

        private Type CollectionTypeOf(object @object)
        {
            return @object.GetType().GetInterfaces()
                .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == ConvertType);
        }
    }
}