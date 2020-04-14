using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(DateTime))]
    public class DateTimeConverter : ValueConverter
    {
        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var dateTime = (DateTime)value;
            return dateTime.Ticks.ToString();
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            var ticks = long.Parse(element.Value);
            return new DateTime(ticks);
        }
    }
}