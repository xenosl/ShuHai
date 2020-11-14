using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    public static class TransformConvert
    {
        private class MemberNames : IEnumerable<string>
        {
            public static readonly MemberNames Instance = new MemberNames();

            public const string Position = "position";
            public const string Rotation = "rotation";
            public const string LocalScale = "localScale";

            public IEnumerator<string> GetEnumerator()
            {
                yield return Position;
                yield return Rotation;
                yield return LocalScale;
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public static void FromXElement(this Transform self, XElement element, XConvertSettings settings = null)
        {
            self.SetMemberValues(element, MemberNames.Instance, settings);
        }

        public static XElement ToXElement(this Transform self, string elementName, XConvertSettings settings = null)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNullOrEmpty(elementName, nameof(elementName));

            var element = new XElement(elementName);
            element.SetChildren(MemberNames.Instance, self, settings);
            return element;
        }
    }
}