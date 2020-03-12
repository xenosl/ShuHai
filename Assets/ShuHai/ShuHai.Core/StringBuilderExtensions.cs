using System;
using System.Text;

namespace ShuHai
{
    public static class StringBuilderExtensions
    {
        #region Insert At Head

        public static StringBuilder InsertHead(this StringBuilder self, bool value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, char value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, byte value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, sbyte value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, short value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, ushort value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, int value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, uint value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, long value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, ulong value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, float value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, double value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, string value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, decimal value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, char[] value) { return self.Insert(0, value); }
        public static StringBuilder InsertHead(this StringBuilder self, object value) { return self.Insert(0, value); }

        #endregion Insert At Head

        #region Remove

        /// <summary>
        ///     Remove specified number of tail characters.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to remove from. </param>
        /// <param name="count"> Number of characters to remove. </param>
        public static StringBuilder RemoveTail(this StringBuilder self, int count)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (count < 0)
                throw new ArgumentException("Negative value is not allowed.", nameof(count));

            var len = self.Length;
            if (count > len)
                throw new ArgumentException($"Count({count}) can not be greater than length({len})", nameof(count));

            self.Remove(len - count, count);
            return self;
        }

        /// <summary>
        ///     Remove all tail line feed characters.
        /// </summary>
        /// <param name="self"> The <see cref="StringBuilder" /> to remove from. </param>
        public static StringBuilder RemoveTailLineFeed(this StringBuilder self)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            int length = self.Length, lastIndex = length - 1;
            if (length > 0 && self[lastIndex] == '\n')
            {
                var nextToLastIndex = lastIndex - 1;
                if (self[nextToLastIndex] == '\r') // The line feed is "\r\n"
                    self.Remove(nextToLastIndex, 2);
                else
                    self.Remove(lastIndex, 1);
            }
            return self;
        }

        #endregion Remove
    }
}