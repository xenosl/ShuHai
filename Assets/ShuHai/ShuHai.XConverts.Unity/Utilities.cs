using System.Collections.Generic;
using System.Linq;
using ShuHai.XConverts.Converters;

namespace ShuHai.XConverts.Unity
{
    internal static class Utilities
    {
        public static string MergeValues(IEnumerable<float> values, ValueStyle style = ValueStyle.Text)
        {
            return string.Join(",", values.Select(v => PrimitiveConverter.ToString(v, style)));
        }

        public static float[] SplitValues(string value, ValueStyle style = ValueStyle.Text)
        {
            return value.Split(',').Select(v => PrimitiveConverter.ToSingle(v, style)).ToArray();
        }
    }
}