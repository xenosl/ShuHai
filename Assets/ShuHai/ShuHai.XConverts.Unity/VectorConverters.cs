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
            return Utilities.MergeValues(new[] { vec.x, vec.y });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Vector2(v[0], v[1]);
        }
    }

    [XConvertType(typeof(Vector3))]
    public class Vector3Converter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector3)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Vector3(v[0], v[1], v[2]);
        }
    }

    [XConvertType(typeof(Vector4))]
    public class Vector4Converter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector4)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z, vec.w });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitSingleValues(element.Value);
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
    }

    [XConvertType(typeof(Vector2Int))]
    public class Vector2IntConverter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector2Int)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitIntValues(element.Value);
            return new Vector2Int(v[0], v[1]);
        }
    }

    [XConvertType(typeof(Vector3Int))]
    public class Vector3IntConverter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector3Int)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z });
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitIntValues(element.Value);
            return new Vector3Int(v[0], v[1], v[2]);
        }
    }
}