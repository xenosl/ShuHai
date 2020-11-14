using System;
using System.Xml.Linq;

namespace ShuHai.XConverts.Converters
{
    [XConvertType(typeof(Type))]
    public class TypeConverter : ValueConverter
    {
        public new static TypeConverter Default { get; } = new TypeConverter();

        protected override string ValueToString(object value, XConvertSettings settings)
        {
            var name = TypeName.Get((Type)value);
            return name.ToString(settings.AssemblyNameStyle);
        }

        protected override object CreateObject(XElement element, Type type, XConvertSettings settings)
        {
            return TypeCache.GetType(element.Value);
        }
    }
}