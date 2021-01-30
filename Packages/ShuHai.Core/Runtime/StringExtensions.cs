using System;
using System.Text;

namespace ShuHai
{
    public static class StringExtensions
    {
        public static string RemoveHead(this string self, int count)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.Remove(0, count);
        }

        /// <summary>
        ///     Get new string in which specified number of characters are removed from the end of current instance.
        /// </summary>
        /// <param name="self">The string instance to be removed.</param>
        /// <param name="count">Number of characters to remove.</param>
        /// <returns>A new string equivalent to current instance without last <paramref name="count" /> characters.</returns>
        public static string RemoveTail(this string self, int count)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.Remove(self.Length - count, count);
        }

        public static bool Contains(this string self, string value, StringComparison comparison)
        {
            return self.IndexOf(value, comparison) >= 0;
        }

        #region Encoding

        public static string ConvertEncoding(this string self, string srcEncodingName, string dstEncodingName)
        {
            var src = Encoding.GetEncoding(srcEncodingName);
            var dst = Encoding.GetEncoding(dstEncodingName);
            return ConvertEncoding(self, src, dst);
        }

        public static string ConvertEncoding(this string self, int srcCodePage, int dstCodePage)
        {
            var src = Encoding.GetEncoding(srcCodePage);
            var dst = Encoding.GetEncoding(dstCodePage);
            return ConvertEncoding(self, src, dst);
        }

        public static string ConvertEncoding(this string self, Encoding srcEncoding, Encoding dstEncoding)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(srcEncoding, nameof(srcEncoding));
            Ensure.Argument.NotNull(dstEncoding, nameof(dstEncoding));

            var srcBytes = srcEncoding.GetBytes(self);
            var dstBytes = Encoding.Convert(srcEncoding, dstEncoding, srcBytes);
            return dstEncoding.GetString(dstBytes);
        }

        #endregion Encoding
    }
}