using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(GameObject))]
    public sealed class GameObjectConverter : ObjectConverter
    {
        public new static GameObjectConverter Default { get; } = new GameObjectConverter();

        private static class MemberNames
        {
            public const string Transform = "transform";

            public const string ActiveSelf = "activeSelf";

            public const string Layer = "layer";
            public const string IsStatic = "isStatic";
            public const string Tag = "tag";

            public static IEnumerable<string> Auto()
            {
                yield return Layer;
                yield return IsStatic;
                yield return Tag;
            }

            public static IEnumerable<string> Manual()
            {
                yield return Transform;
                yield return ActiveSelf;
            }

            public static IEnumerable<string> All() { return Auto().Concat(Manual()); }
        }

        #region Object To XElement

        protected override void PopulateXElementChildren(XElement element, object @object, XConvertSettings settings)
        {
            base.PopulateXElementChildren(element, @object, settings);

            var obj = (GameObject)@object;

            element.Add(obj.transform.ToXElement(MemberNames.Transform, settings));

            element.SetChildren(MemberNames.Auto(), @object, settings);
            element.Add(obj.activeSelf.ToXElement(MemberNames.ActiveSelf, settings));

            PopulateXElementComponents(element, obj, settings);
        }

        private void PopulateXElementComponents(XElement element, GameObject @object, XConvertSettings settings)
        {
            // TODO: Implements component converter.
        }

        #endregion Object To XElement

        #region XElement To Object

        public GameObject ToGameObject(XElement element,
            XConvertSettings settings = null, XConvertToObjectSession session = null)
        {
            return (GameObject)ToObject(element, settings, session);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            Debug.Assert(type == typeof(GameObject));
            return new GameObject();
        }

        protected override void PopulateObjectMembers(object @object, XElement element, XConvertSettings settings)
        {
            base.PopulateObjectMembers(@object, element, settings);

            var obj = (GameObject)@object;

            obj.transform.FromXElement(element.Element(MemberNames.Transform), settings);

            obj.SetMemberValues(element, MemberNames.Auto(), settings);
            obj.SetActive((bool)element.Element(MemberNames.ActiveSelf).ToObject(settings));

            PopulateObjectComponents(obj, element, settings);
        }

        private void PopulateObjectComponents(GameObject @object, XElement element, XConvertSettings settings)
        {
            // TODO: Implements component converter.
        }

        #endregion XElement To Object
    }
}