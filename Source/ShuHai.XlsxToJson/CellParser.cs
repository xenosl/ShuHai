using System;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public abstract class CellParser
    {
        public abstract JToken Parse(object value, object fallbackValue, out Exception exception);

        public JToken Parse(object value, object fallbackValue = null) { return Parse(value, fallbackValue, out _); }
    }
}
