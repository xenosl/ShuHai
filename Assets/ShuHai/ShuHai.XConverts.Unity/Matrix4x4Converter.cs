using System;
using System.Xml.Linq;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity
{
    [XConvertType(typeof(Matrix4x4))]
    public sealed class Matrix4x4Converter : ValueConverter
    {
        public new static Matrix4x4Converter Default { get; } = new Matrix4x4Converter();

        private const int ElementCount = 16;

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var m = (Matrix4x4)value;
            var values = new float[ElementCount];
            for (int i = 0; i < ElementCount; ++i)
                values[i] = m[i];
            return Utilities.MergeValues(values);
        }

        public Matrix4x4 ToMatrix4x4(XElement element, XConvertSettings settings = null)
        {
            return (Matrix4x4)base.ToObject(element, settings);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var values = Utilities.SplitSingleValues(element.Value);
            var m = new Matrix4x4();
            for (int i = 0; i < ElementCount; ++i)
                m[i] = values[i];
            return m;
        }
    }
}