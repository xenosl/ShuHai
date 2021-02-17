using System;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public class StringCellParser : CellParser
    {
        public override JToken Parse(object value, object fallbackValue, out Exception exception)
        {
            exception = null;
            switch (value)
            {
                case null:
                case DBNull _:
                    return new JValue(string.Empty);
                case string strValue:
                    return new JValue(strValue);
                default:
                    exception = new NotSupportedException();
                    return new JValue(fallbackValue);
            }
        }
    }
}
