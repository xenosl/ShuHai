using System;
using System.Globalization;

namespace ShuHai.SConverts.Converters
{
    public class DecimalConverter : SConverter<Decimal>
    {
        public static readonly DecimalConverter Instance = new DecimalConverter();

        public override string ToString(decimal value) { return value.ToString(CultureInfo.InvariantCulture); }

        public override decimal ToValue(string str) { return Decimal.Parse(str); }

        private DecimalConverter() { }
    }
}