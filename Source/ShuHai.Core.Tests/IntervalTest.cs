using System;
using NUnit.Framework;

namespace ShuHai.Tests
{
    public class IntervalTest
    {
        [Test]
        public void Union()
        {
            Union(2, 8, 4, 8, 2, 7);
            Union(2, 8, 7, 2, 4, 8);
            Union(2, 4, 2, 2, 4, 4);
        }

        private static void Union<T>(T expectL, T expectR, T i1L, T i1R, T i2L, T i2R)
        {
            var expect = new Interval<T>(expectL, expectR);
            Interval<T> i1 = new Interval<T>(i1L, i1R), i2 = new Interval<T>(i2L, i2R);
            Assert.AreEqual(expect, Interval<T>.Union(i1, i2));
        }

        [Test]
        public void Clamp()
        {
            Clamp(2, 8, 2, 8, 0, 33);
            Clamp(5, 8, 2, 8, 5, 33);
            Clamp(8, 5, 8, 2, 33, 5);
            Clamp(8, 8, 8, 8, 2, 5);
        }

        private static void Clamp<T>(T expectL, T expectR, T l, T r, T targetL, T targetR)
        {
            var expect = new Interval<T>(expectL, expectR);
            Interval<T> i = new Interval<T>(l, r), target = new Interval<T>(targetL, targetR);
            Assert.AreEqual(expect, i.Clamp(target));
        }

        [Test]
        public void Contains()
        {
            // Contains values
            Contains(true, 1, 3, 1, true);
            Contains(true, 1, 3, 3, true);
            Contains(false, 1, 3, 3, false);
            Contains(true, 1, 1, 1, true);

            Contains(false, 0.2, 1.2, 1.2, false);
            Contains(true, 0.2, 1.2, 0.2, true);
            Contains(true, 0.2, 1.2, 1.2, true);

            // Contains intervals
            Contains(true, 1, 3, 1, 3, true);
            Contains(false, 1, 3, 1, 3, false);
            Contains(true, 1, 3, 2, 2, true);
            Contains(true, 1, 3, 3, 3, true);
        }

        private static void Contains<T>(bool expect, T l, T r, T value, bool closed)
            where T : IComparable<T>
        {
            var interval = new Interval<T>(l, r);
            Assert.AreEqual(expect, interval.Contains(value, closed));
        }

        private static void Contains<T>(bool expect, T l, T r, T otherL, T otherR, bool closed)
        {
            Interval<T> interval = new Interval<T>(l, r), other = new Interval<T>(otherL, otherR);
            Assert.AreEqual(expect, interval.Contains(other, closed));
        }

        [Test]
        public void Length()
        {
            Length(10, 2, 12);
            Length(12.3 - 2.2331, 2.2331, 12.3);
            Length(12.3 - 2.2331, 12.3, 2.2331);
            Length(0, 2, 2);
        }

        private static void Length<T>(T expect, T min, T max)
        {
            var interval = new Interval<T>(min, max);
            Assert.AreEqual(expect, interval.Length);
        }
    }
}