using System;
using System.Linq;
using System.Xml.Linq;
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

        protected static string MergeValues(params float[] values) { return string.Join(",", values); }

        protected static float[] SplitValues(string value) { return value.Split(',').Select(float.Parse).ToArray(); }
    }

    [XConvertType(typeof(Vector2))]
    public class Vector2Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector2)@object;
            element.Value = MergeValues(value.x, value.y);
        }

        protected override object CreateObject(XElement element, Type type)
        {
            var v = SplitValues(element.Value);
            return new Vector2(v[0], v[1]);
        }
    }

    [XConvertType(typeof(Vector3))]
    public class Vector3Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector3)@object;
            element.Value = MergeValues(value.x, value.y, value.z);
        }

        protected override object CreateObject(XElement element, Type type)
        {
            var v = SplitValues(element.Value);
            return new Vector3(v[0], v[1], v[2]);
        }
    }

    [XConvertType(typeof(Vector4))]
    public class Vector4Converter : VectorConverter
    {
        protected override void PopulateXElementValue(XElement element, object @object, XConvertSettings settings)
        {
            var value = (Vector4)@object;
            element.Value = MergeValues(value.x, value.y, value.z, value.w);
        }

        protected override object CreateObject(XElement element, Type type)
        {
            var v = SplitValues(element.Value);
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
    }
}