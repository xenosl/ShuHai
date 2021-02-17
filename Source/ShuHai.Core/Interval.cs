using System;
using System.Collections.Generic;

namespace ShuHai
{
    public enum IntervalDirection
    {
        /// <summary>
        ///     <see cref="Interval{T}.L" /> is the lower boundary with minimum value and <see cref="Interval{T}.R" /> is
        ///     the higher boundary with maximum value.
        /// </summary>
        L2R,

        /// <summary>
        ///     <see cref="Interval{T}.R" /> is the lower boundary with minimum value and <see cref="Interval{T}.L" /> is
        ///     the higher boundary with maximum value.
        /// </summary>
        R2L
    }

    /// <summary>
    ///     Represents a value interval defined by two boundary values.
    /// </summary>
    /// <typeparam name="T">Type of the boundary value.</typeparam>
    public struct Interval<T> : IEquatable<Interval<T>>
    {
        /// <summary>
        ///     Direction used when setting <see cref="L" /> and/or <see cref="R" /> boundaries with undetermined condition.
        ///     For instance, setting <see cref="Min" /> when the interval has no direction (which means <see cref="L" />
        ///     and <see cref="R" /> are the same), this default direction is used to determine which boundary is set.
        /// </summary>
        public static IntervalDirection DefaultDirection = IntervalDirection.L2R;

        /// <summary>
        ///     Comparer instance used to decide the relation between two values of type <see cref="T" />.
        /// </summary>
        /// <remarks>
        ///     The comparer of primitive types and some .Net built-in types are already defined by default. You have to
        ///     specify your own comparer if the field instance is missing.
        /// </remarks>
        public static IComparer<T> Comparer = IntervalComparers.Get<T>();

        /// <summary>
        ///     Subtract function for <see cref="T" /> used in current type.
        /// </summary>
        /// <remarks>
        ///     The function is currently used by <see cref="Length" /> property and exception is thrown if the function
        ///     is not assigned. The function for primitive types are already assigned by default, you have to assign your
        ///     own subtract function for custom types to make <see cref="Length" /> properly work.
        /// </remarks>
        public static Func<T, T, T> Subtractor = IntervalSubtractors.Get<T>();

        /// <summary>
        ///     Left boundary of current interval.
        /// </summary>
        public T L;

        /// <summary>
        ///     Right boundary of current interval.
        /// </summary>
        public T R;

        /// <summary>
        ///     Boundary direction of current interval.
        ///     The <see langword="null" /> value means the left and right value of the boundary is the same (ie. no direction).
        /// </summary>
        public IntervalDirection? Direction
        {
            get
            {
                if (Comparer.Greater(R, L))
                    return IntervalDirection.L2R;
                if (Comparer.Less(R, L))
                    return IntervalDirection.R2L;
                return null;
            }
        }

        /// <summary>
        ///     Length between boundary <see cref="L" /> and <see cref="R" />.
        /// </summary>
        public T Length => Subtractor(Max, Min);

        public Interval<T> Inversed => new Interval<T>(R, L);

        /// <summary>
        ///     Initialize a interval with its two boundaries.
        /// </summary>
        /// <param name="l">Left boundary of the interval.</param>
        /// <param name="r">Right boundary of the interval.</param>
        public Interval(T l, T r)
        {
            L = l;
            R = r;
        }

        #region Min & Max

        /// <summary>
        ///     Boundary with the minimum value of current interval.
        /// </summary>
        /// <remarks>
        ///     Setting the minimum value requires the given value less than <see cref="Max" />, otherwise, it take no
        ///     effect.
        /// </remarks>
        public T Min
        {
            get => Direction == IntervalDirection.L2R ? L : R;
            set => SetMin(value);
        }

        /// <summary>
        ///     Boundary with the maximum value of current interval.
        /// </summary>
        /// <remarks>
        ///     Setting the maximum value requires the given value greater than <see cref="Min" />, otherwise, it take no
        ///     effect.
        /// </remarks>
        public T Max
        {
            get => Direction == IntervalDirection.L2R ? R : L;
            set => SetMax(value);
        }

        /// <summary>
        ///     Set the boundary value of minimum side.
        /// </summary>
        /// <param name="value">The new minimum value.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value successfully set; otherwise, <see langword="false" /> if
        ///     the specified value is greater than <see cref="Max" /> boundary value of current interval.
        /// </returns>
        public bool SetMin(T value)
        {
            if (Comparer.Greater(value, Max))
                return false;

            switch (Direction)
            {
                case null:
                    switch (DefaultDirection)
                    {
                        case IntervalDirection.L2R:
                            L = value;
                            break;
                        case IntervalDirection.R2L:
                            R = value;
                            break;
                        default:
                            throw new EnumOutOfRangeException(DefaultDirection);
                    }
                    break;
                case IntervalDirection.L2R:
                    L = value;
                    break;
                case IntervalDirection.R2L:
                    R = value;
                    break;
                default:
                    throw new EnumOutOfRangeException(Direction);
            }

            return true;
        }

        /// <summary>
        ///     Set the boundary value of maximum side.
        /// </summary>
        /// <param name="value">The new maximum value.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value successfully set; otherwise, <see langword="false" /> if
        ///     the specified value is less than <see cref="Min" /> boundary value of current interval.
        /// </returns>
        public bool SetMax(T value)
        {
            if (Comparer.Less(value, Min))
                return false;

            switch (Direction)
            {
                case null:
                    switch (DefaultDirection)
                    {
                        case IntervalDirection.L2R:
                            R = value;
                            break;
                        case IntervalDirection.R2L:
                            L = value;
                            break;
                        default:
                            throw new EnumOutOfRangeException(DefaultDirection);
                    }
                    break;
                case IntervalDirection.L2R:
                    R = value;
                    break;
                case IntervalDirection.R2L:
                    L = value;
                    break;
                default:
                    throw new EnumOutOfRangeException(Direction);
            }

            return true;
        }

        #endregion Min & Max

        /// <summary>
        ///     Switch the two boundaries <see cref="L" /> and <see cref="R" /> of current interval.
        /// </summary>
        public void Inverse()
        {
            var t = L;
            L = R;
            R = t;
        }

        public void Extend(T boundary)
        {
            if (Comparer.Less(boundary, Min))
                Min = boundary;
            else if (Comparer.Greater(boundary, Max))
                Max = boundary;
        }

        /// <summary>
        ///     Clamps the boundaries of the specified interval into range of current interval.
        /// </summary>
        /// <param name="target">The interval to clamp.</param>
        /// <returns>The <paramref name="target" /> interval clamped by current interval.</returns>
        public Interval<T> Clamp(Interval<T> target)
        {
            if (Comparer.GreaterOrEqual(target.Min, Max))
            {
                target.L = Max;
                target.R = Max;
                return target;
            }

            if (Comparer.LessOrEqual(target.Max, Min))
            {
                target.L = Min;
                target.R = Min;
                return target;
            }

            target.Min = MathEx.Max(target.Min, Min, Comparer);
            target.Max = MathEx.Min(target.Max, Max, Comparer);
            return target;
        }

        /// <summary>
        ///     Clamps the specified value into range of current interval.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        public T Clamp(T value)
        {
            if (Comparer.Less(value, Min))
                value = Min;
            if (Comparer.Greater(value, Max))
                value = Max;
            return value;
        }

        public static Interval<T> Union(Interval<T> i1, Interval<T> i2)
        {
            var dir = i1.Direction == i2.Direction ? i1.Direction : IntervalDirection.L2R;
            T min = MathEx.Min(i1.Min, i2.Min, Comparer), max = MathEx.Max(i1.Max, i2.Max, Comparer);

            T l, r;
            switch (dir)
            {
                case null:
                    switch (DefaultDirection)
                    {
                        case IntervalDirection.L2R:
                            l = min;
                            r = max;
                            break;
                        case IntervalDirection.R2L:
                            r = min;
                            l = max;
                            break;
                        default:
                            throw new EnumOutOfRangeException(DefaultDirection);
                    }
                    break;
                case IntervalDirection.L2R:
                    l = min;
                    r = max;
                    break;
                case IntervalDirection.R2L:
                    r = min;
                    l = max;
                    break;
                default:
                    throw new EnumOutOfRangeException(dir);
            }
            return new Interval<T>(l, r);
        }

        /// <summary>
        ///     Get a value indicates whether the specified value lies inside the range of current interval.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <param name="closed">Indicates whether to include boundary when testing.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value lies inside the range of current interval; otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool Contains(T value, bool closed = true)
        {
            return closed
                ? Comparer.GreaterOrEqual(value, Min) && Comparer.LessOrEqual(value, Max)
                : Comparer.Greater(value, Min) && Comparer.Less(value, Max);
        }

        /// <summary>
        ///     Get a value indicates whether the specified interval lies inside the range of current interval.
        /// </summary>
        /// <param name="other">The interval to test.</param>
        /// <param name="closed">Indicates whether to include boundary when testing.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified interval lies inside the range of current interval; otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool Contains(Interval<T> other, bool closed = true)
        {
            return Contains(other.Min, closed) && Contains(other.Max, closed);
        }

        /// <summary>
        ///     Get a value indicates whether the specified interval overlaps with current interval.
        /// </summary>
        /// <param name="other">The interval to test.</param>
        /// <param name="closed">Indicates whether to include boundary when testing.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified interval overlaps with current interval; otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool Overlaps(Interval<T> other, bool closed = true)
        {
            return Contains(other.L, closed) || Contains(other.R, closed);
        }

        public override string ToString() { return $"[{L}, {R}]"; }

        #region Equality

        /// <summary>
        ///     Indicates whether the specified interval represents the same range as current interval.
        /// </summary>
        /// <param name="other">The interval to compare.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified interval represents the same range as current interval; otherwise
        ///     <see langword="false" />.
        /// </returns>
        public bool RangeEquals(Interval<T> other)
        {
            return Comparer.Equal(L, other.L) && Comparer.Equal(R, other.R)
                || Comparer.Equal(L, other.R) && Comparer.Equal(R, other.L);
        }

        public bool Equals(Interval<T> other) { return Comparer.Equal(L, other.L) && Comparer.Equal(R, other.R); }

        public override bool Equals(object obj) { return obj is Interval<T> other && Equals(other); }

        public override int GetHashCode() { return HashCode.Get(L, R); }

        public static bool operator ==(Interval<T> x, Interval<T> y) { return x.Equals(y); }
        public static bool operator !=(Interval<T> x, Interval<T> y) { return !(x == y); }

        #endregion Equality
    }

    /// <summary>
    ///     Provides default comparers for <see cref="Interval{T}.Comparer" />.
    /// </summary>
    internal static class IntervalComparers
    {
        public static void Add<T>(IComparer<T> comparer) { comparers.Add(typeof(T), comparer); }

        public static IComparer<T> Get<T>() { return (IComparer<T>)comparers.GetValue(typeof(T)); }

        private static readonly Dictionary<Type, object> comparers = new Dictionary<Type, object>();

        static IntervalComparers()
        {
            Add(Comparer<Boolean>.Default);
            Add(Comparer<Char>.Default);
            Add(Comparer<SByte>.Default);
            Add(Comparer<Byte>.Default);
            Add(Comparer<Int16>.Default);
            Add(Comparer<UInt16>.Default);
            Add(Comparer<Int32>.Default);
            Add(Comparer<UInt32>.Default);
            Add(Comparer<Int64>.Default);
            Add(Comparer<UInt64>.Default);
            Add(Comparer<Single>.Default);
            Add(Comparer<Double>.Default);
            Add(Comparer<Decimal>.Default);
            Add(Comparer<DateTime>.Default);
        }
    }

    /// <summary>
    ///     Provides default subtractors for <see cref="Interval{T}.Subtractor" />.
    /// </summary>
    internal static class IntervalSubtractors
    {
        public static void Add<T>(Func<T, T, T> subtractor) { subtractors.Add(typeof(T), subtractor); }

        public static Func<T, T, T> Get<T>() { return (Func<T, T, T>)subtractors.GetValue(typeof(T)); }

        private static readonly Dictionary<Type, object> subtractors = new Dictionary<Type, object>();

        static IntervalSubtractors()
        {
            Add<SByte>((l, r) => (SByte)(l - r));
            Add<Byte>((l, r) => (Byte)(l - r));
            Add<Int16>((l, r) => (Int16)(l - r));
            Add<UInt16>((l, r) => (UInt16)(l - r));
            Add<Int32>((l, r) => l - r);
            Add<UInt32>((l, r) => l - r);
            Add<Int64>((l, r) => l - r);
            Add<UInt64>((l, r) => l - r);
            Add<Single>((l, r) => l - r);
            Add<Double>((l, r) => l - r);
            Add<Decimal>((l, r) => l - r);
        }
    }
}