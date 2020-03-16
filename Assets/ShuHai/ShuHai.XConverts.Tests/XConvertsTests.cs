﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.XConverts
{
    public class XConvertsTests
    {
        [Test]
        public void ObjectConverterTest()
        {
            var c = XConverter.Default;

            ConvertTest(c, null);
            ConvertTest(c, 2);
            ConvertTest(c, 4.231f);
            ConvertTest(c, 931.5523358);
            ConvertTest(c, Guid.NewGuid());

            ConvertTest(c, 'c');
            ConvertTest(c, '$');
            ConvertTest(c, ' ');
            ConvertTest(c, '\\');
            ConvertTest(c, '\t');

            ConvertTest(c, 0);
            ConvertTest(c, 12);
            ConvertTest(c, -22);
            ConvertTest(c, int.MinValue);
            ConvertTest(c, int.MaxValue);

            ConvertTest(c, 0f);
            ConvertTest(c, 12.122f);
            ConvertTest(c, -22.001f);
            ConvertTest(c, float.NaN);
            ConvertTest(c, float.Epsilon);
            //ConvertTest(c, float.MinValue);
            //ConvertTest(c, float.MaxValue);
        }

        [Test]
        public void StringConverterTest()
        {
            var c = XConverter.BuiltIns[typeof(string)];
            ConvertTest(c, null);
            ConvertTest(c, string.Empty);
            ConvertTest(c, "This is a string.");
        }

        [Test]
        public void CollectionConverterTest()
        {
            var c = XConverter.BuiltIns[typeof(ICollection<>)];
            var c0 = new HashSet<string>();
            var c1 = new List<object> { 978.44, Guid.NewGuid() };
            var c2 = new Dictionary<int, object> { { 1, "string item" }, { 2, 231 }, { 3, 5235.11 }, { 4, c1 } };

            ConvertTest(c, c0);
            ConvertTest(c, c1);
            ConvertTest(c, c2);
        }

        private static void ConvertTest(XConverter converter, object value)
        {
            var element = converter.ToXElement(value, "ConvertTest");
            var obj = converter.ToObject(element);
            Assert.AreEqual(value, obj);
        }
    }
}