using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Vector2))]
    public class Vector2Converter : ValueConverter
    {
        public new static Vector2Converter Default { get; } = new Vector2Converter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector2)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y });
        }

        public Vector2 ToVector2(XElement element, XConvertSettings settings = null)
        {
            return (Vector2)base.ToObject(element, settings);
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
        public new static Vector3Converter Default { get; } = new Vector3Converter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector3)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z });
        }

        public Vector3 ToVector3(XElement element, XConvertSettings settings = null)
        {
            return (Vector3)base.ToObject(element, settings);
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
        public new static Vector4Converter Default { get; } = new Vector4Converter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector4)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z, vec.w });
        }

        public Vector4 ToVector4(XElement element, XConvertSettings settings = null)
        {
            return (Vector4)base.ToObject(element, settings);
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
        public new static Vector2IntConverter Default { get; } = new Vector2IntConverter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector2Int)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y });
        }

        public Vector2Int ToVector2Int(XElement element, XConvertSettings settings = null)
        {
            return (Vector2Int)base.ToObject(element, settings);
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
        public new static Vector3IntConverter Default { get; } = new Vector3IntConverter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var vec = (Vector3Int)value;
            return Utilities.MergeValues(new[] { vec.x, vec.y, vec.z });
        }

        public Vector3Int ToVector3Int(XElement element, XConvertSettings settings = null)
        {
            return (Vector3Int)base.ToObject(element, settings);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = Utilities.SplitIntValues(element.Value);
            return new Vector3Int(v[0], v[1], v[2]);
        }
    }
}