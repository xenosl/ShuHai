using System;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public class IntegerCellParser : CellParser
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
                    if (int.TryParse(strValue, out var i))
                        return new JValue(i);
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
                case int intValue:
                    return new JValue(intValue);
                default:
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
            }
        }
    }
}
