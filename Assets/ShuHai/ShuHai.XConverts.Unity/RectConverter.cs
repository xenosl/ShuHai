using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Rect))]
    public sealed class RectConverter : ValueConverter
    {
        public new static RectConverter Default { get; } = new RectConverter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var r = (Rect)value;
            return Utilities.MergeValues(new[] { r.x, r.y, r.width, r.height });
        }

        public Rect ToRect(XElement element, XConvertSettings settings = null)
        {
            return (Rect)base.ToObject(element, settings);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Rect(v[0], v[1], v[2], v[3]);
        }
    }
}