using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.XConverts.Converters;

namespace ShuHai.XConverts.Unity
{
    internal static class Utilities
    {
        public static string MergeValues(IEnumerable<int> values, ValueStyle style = ValueStyle.Text)
        {
            return MergeValues(values, ValueConverter.ToString, style);
        }

        public static string MergeValues(IEnumerable<float> values, ValueStyle style = ValueStyle.Text)
        {
            return MergeValues(values, ValueConverter.ToString, style);
        }
        
        public static int[] SplitIntValues(string value, ValueStyle style = ValueStyle.Text)
        {
            return SplitValues(value, ValueConverter.ToInt32, style);
        }

        public static float[] SplitSingleValues(string value, ValueStyle style = ValueStyle.Text)
        {
            return SplitValues(value, ValueConverter.ToSingle, style);
        }

        private static string MergeValues<T>(IEnumerable<T> values,
            Func<T, ValueStyle, string> valueToString, ValueStyle style)
        {
            return string.Join(",", values.Select(v => valueToString(v, style)));
        }

        private static T[] SplitValues<T>(string value, Func<string, ValueStyle, T> stringToValue, ValueStyle style)
        {
            return value.Split(',').Select(v => stringToValue(v, style)).ToArray();
        }
    }
}