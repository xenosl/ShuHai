using System;
using System.Text.RegularExpressions;
using ShuHai.Unity;
using UnityEngine;

namespace ShuHai.SConverts.Unity
{
    public sealed class ColorConverter : SConverter<Color>
    {
        public static readonly ColorConverter Default = new ColorConverter();

        public override string ToString(Color value) { return value.ToColor32().ToString(); }

        public override Color ToValue(string str)
        {
            Ensure.Argument.NotNullOrEmpty(str, nameof(str));

            var match = Regex.Match(str, RegexPattern, RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new ArgumentException("The specified string does not represents a color value.");

            var mg = match.Groups;
            byte a = mg[4].Success ? ToByte(mg[5]) : byte.MaxValue;
            return Colors.New(ToByte(mg[1]), ToByte(mg[2]), ToByte(mg[3]), a);
        }

        //language=regexp
        private const string RegexPattern = @"RGBA\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*(,\s*(\d+)\s*)?\)";

        private static byte ToByte(Group group) { return byte.Parse(group.Value); }
    }
}