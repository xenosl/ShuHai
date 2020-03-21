using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    public class QuaternionConverter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var q = (Quaternion)value;
            return Utilities.MergeValues(new[] { q.x, q.y, q.z, q.w });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitValues(element.Value);
            return new Quaternion(v[0], v[1], v[2], v[3]);
        }
    }
}