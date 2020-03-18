using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Vector2))]
    public class Vector2Converter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector2)value;
            return MergeValues(new[] { vec.x, vec.y }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector2(v[0], v[1]);
        }
    }

    [XConvertType(typeof(Vector3))]
    public class Vector3Converter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector3)value;
            return MergeValues(new[] { vec.x, vec.y, vec.z }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector3(v[0], v[1], v[2]);
        }
    }

    [XConvertType(typeof(Vector4))]
    public class Vector4Converter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector4)value;
            return MergeValues(new[] { vec.x, vec.y, vec.z, vec.w }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
    }
}