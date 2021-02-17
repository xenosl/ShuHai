using System;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public class FloatCellParser : CellParser
    {
        public override JToken Parse(object value, object fallbackValue, out Exception exception)
        {
            exception = null;
            switch (value)
            {
                case null:
                case DBNull _:
                    return new JValue(fallbackValue);
                case string strValue:
                    if (float.TryParse(strValue, out var f))
                        return new JValue(f);
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
                case int intValue:
                    return new JValue((double)intValue);
                case float floatValue:
                    return new JValue(floatValue);
                case double doubleValue:
                    return new JValue(doubleValue);
                default:
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
            }
        }
    }
}
