using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, item, itemType);
                var childElement = converter.ToXElement(item, "Item", settings);
                element.Add(childElement);
            }
        }

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
                    var converter = XConverterSelector.SelectWithBuiltins(settings.Converters, itemType);
                    item = converter.ToObject(childElement, settings);
                }
                addMethod.Invoke(@object, new[] { item });
            }
        }

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
    }
}