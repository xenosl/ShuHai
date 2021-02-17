using System;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public class BoolCellParser : CellParser
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
                    if (bool.TryParse(strValue, out var b))
                        return new JValue(b);
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
                case int intValue:
                    return new JValue(intValue != 0);
                case float floatValue:
                    return new JValue(floatValue != 0);
                case double doubleValue:
                    return new JValue(doubleValue != 0);
                case bool boolValue:
                    return new JValue(boolValue);
                default:
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
            }
        }
    }
}
