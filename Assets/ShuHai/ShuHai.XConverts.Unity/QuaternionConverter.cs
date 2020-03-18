﻿using System;
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
            return MergeValues(new[] { q.x, q.y, q.z, q.w }, settings.FloatingPointStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var v = SplitValues(element.Value, settings.FloatingPointStyle);
            return new Quaternion(v[0], v[1], v[2], v[3]);
        }
    }
}