using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Quaternion))]
    public sealed class QuaternionConverter : ValueConverter
    {
        public new static QuaternionConverter Default { get; } = new QuaternionConverter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var q = (Quaternion)value;
            return Utilities.MergeValues(new[] { q.x, q.y, q.z, q.w });
        }

        public Quaternion ToQuaternion(XElement element, XConvertSettings settings = null)
        {
            return (Quaternion)base.ToObject(element, settings);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Quaternion(v[0], v[1], v[2], v[3]);
        }
    }
}