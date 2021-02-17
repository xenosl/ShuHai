using System;
using System.Collections.Generic;

namespace ShuHai
{
    /// <summary>
    ///     Extra methods for <see cref="Math" />.
    /// </summary>
    public static class MathEx
    {
        public static T Min<T>(T x, T y) where T : IComparable<T> { return x.LessThan(y) ? x : y; }

        public static T Max<T>(T x, T y) where T : IComparable<T> { return x.GreaterThan(y) ? x : y; }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.LessThan(min))
                value = min;
            if (value.GreaterThan(max))
                value = max;
            return value;
        }

        public static T Min<T>(T x, T y, IComparer<T> comparer)
        {
            Ensure.Argument.NotNull(comparer, nameof(comparer));
            return comparer.Compare(x, y) < 0 ? x : y;
        }

        public static T Max<T>(T x, T y, IComparer<T> comparer)
        {
            Ensure.Argument.NotNull(comparer, nameof(comparer));
            return comparer.Compare(x, y) > 0 ? x : y;
        }

        public static T Clamp<T>(T value, T min, T max, IComparer<T> comparer)
        {
            Ensure.Argument.NotNull(comparer, nameof(comparer));

            if (comparer.Compare(value, min) < 0)
                value = min;
            if (comparer.Compare(value, max) > 0)
                value = max;
            return value;
        }
        
        #region Distribution

        /// <summary>
        ///     Returns the standard normal cumulative distribution
        ///     function. The distribution has a mean of 0 (zero) and
        ///     a standard deviation of one. Use this function in place
        ///     of a table of standard normal curve areas.
        /// </summary>
        /// <param name="zValue">The value for which you want the distribution.</param>
        /// <returns>Returns the standard normal cumulative distribution.</returns>
        public static double NormalDistribution(double zValue)
        {
            double[] a = { 0.31938153, -0.356563782, 1.781477937, -1.821255978, 1.330274429 };
            double result;
            if (zValue < -7.0)
            {
                result = NormalDistributionFunction(zValue) / Math.Sqrt(1.0 + zValue * zValue);
            }
            else if (zValue > 7.0)
            {
                result = 1.0 - NormalDistribution(-zValue);
            }
            else
            {
                result = 0.2316419;
                result = 1.0 / (1 + result * Math.Abs(zValue));
                result = 1 - NormalDistributionFunction(zValue)
                    * (result * (a[0] + result * (a[1] + result * (a[2] + result * (a[3] + result * a[4])))));
                if (zValue <= 0.0)
                    result = 1.0 - result;
            }
            return result;
        }

        /// <summary>
        ///     Standard normal density function
        /// </summary>
        /// <param name="t">T Value</param>
        /// <returns>Standard normal density</returns>
        private static double NormalDistributionFunction(double t) { return 0.398942280401433 * Math.Exp(-t * t / 2); }

        #endregion Distribution
    }
}