using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Rect))]
    public class RectConverter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var r = (Rect)value;
            return Utilities.MergeValues(new[] { r.x, r.y, r.width, r.height });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Rect(v[0], v[1], v[2], v[3]);
        }
    }
}