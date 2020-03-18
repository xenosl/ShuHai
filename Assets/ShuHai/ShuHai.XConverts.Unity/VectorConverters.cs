using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    public abstract class VectorConverter : XConverter
    {
        protected sealed override void PopulateXElementChildren(
            XElement element, object @object, XConvertSettings settings)
        {
            // Nothing to do...
        }

        protected sealed override void PopulateObjectMembersImpl(
            object @object, XElement element, XConvertSettings settings)
        {
            // Nothing to do...
        }

        protected static string MergeValues(IEnumerable<float> values, ValueStyle style)
        {
            return string.Join(",", values.Select(v => PrimitiveConverter.ToString(v, style)));
        }

        protected static float[] SplitValues(string value, ValueStyle style)
        {
            return value.Split(',').Select(v => PrimitiveConverter.ToSingle(v, style)).ToArray();
        }

        private static XConverter ValueConverter => _valueConverter.Value;

        private static readonly Lazy<XConverter>
            _valueConverter = new Lazy<XConverter>(() => BuiltIns[typeof(float)]);
    }

    [XConvertType(typeof(Vector2))]
    public class Vector2Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector2)@object;
            element.Value = MergeValues(new[] { value.x, value.y }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector2(v[0], v[1]);
        }
    }

    [XConvertType(typeof(Vector3))]
    public class Vector3Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector3)@object;
            element.Value = MergeValues(new[] { value.x, value.y, value.z }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector3(v[0], v[1], v[2]);
        }
    }

    [XConvertType(typeof(Vector4))]
    public class Vector4Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector4)@object;
            element.Value = MergeValues(new[] { value.x, value.y, value.z, value.w }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
    }
}