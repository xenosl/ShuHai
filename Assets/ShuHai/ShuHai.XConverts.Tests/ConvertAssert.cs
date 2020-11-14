using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace ShuHai.XConverts
{
    public static class ConvertAssert
    {
        public static void ConvertedEqual(this XConverter converter,
            object value, XConvertSettings settings = null, IEqualityComparer comparer = null)
        {
            comparer = comparer ?? EqualityComparer<object>.Default;
            ConvertedEqual(converter, value, settings, comparer.ToGeneric<object>());
        }

        public static void ConvertedEqual<T>(this XConverter converter,
            T value, XConvertSettings settings = null, IEqualityComparer<T> comparer = null)
        {
            settings = settings ?? XConvertSettings.Default;
            comparer = comparer ?? EqualityComparer<T>.Default;

            var convertedElement = converter.ToXElement(value, "ConvertedEqual", settings);
            var convertedValue = (T)converter.ToObject(convertedElement, settings);
            Assert.IsTrue(comparer.Equals(value, convertedValue));
        }
    }
}