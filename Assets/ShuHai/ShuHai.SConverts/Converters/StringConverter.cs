using System;

namespace ShuHai.SConverts.Converters
{
    public class StringConverter : SConverter<String>
    {
        public static readonly StringConverter Instance = new StringConverter();

        public override string ToString(string value) { return value; }

        public override string ToValue(string str) { return str; }

        private StringConverter() { }
    }
}