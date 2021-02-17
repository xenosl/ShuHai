using System;

namespace ShuHai
{
    /// <summary>
    ///     Extra methods of <see cref="Array" />.
    /// </summary>
    public static class ArrayEx
    {
        #region New

        /// <summary>
        ///     Create an array and populate with specified value.
        /// </summary>
        /// <typeparam name="T">Element type of the array.</typeparam>
        /// <param name="length">Length of the array.</param>
        /// <param name="value">Value that populate to the array.</param>
        public static T[] New<T>(int length, T value = default)
        {
            var array = new T[length];
            for (var i = 0; i < length; ++i)
                array[i] = value;
            return array;
        }

        public static T[] New<T>(int length, Func<int, T> valueFactory)
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            var array = new T[length];
            for (var i = 0; i < length; ++i)
                array[i] = valueFactory(i);
            return array;
        }

        #endregion New
    }
}
