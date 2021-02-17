using System;

namespace ShuHai
{
    public static class Primitives
    {
        public static float DefaultFloatTolerance = 1e-10f;

        public static bool AlmostEquals(this float self, float other)
        {
            return AlmostEquals(self, other, DefaultFloatTolerance);
        }

        public static bool AlmostEquals(this float self, float other, float tolerance)
        {
            // wrong - don't do this!
            // A fixed tolerance chosen because it “looks small” could actually be way too large when
            // the numbers being compared are very small as well.
            //return (Math.Abs(self - other) <= tolerance);

            if (self == other)
            {
                // shortcut, handles infinities
                return true;
            }

            float diff = Math.Abs(self - other);
            if (self == 0 || other == 0 || diff < float.Epsilon)
            {
                // self or other is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < tolerance;
            }

            float selfAbs = Math.Abs(self);
            float otherAbs = Math.Abs(other);
            // use relative error
            return diff / (selfAbs + otherAbs) < tolerance;
        }

        public static float DefaultDoubleTolerance = 1e-10f;

        public static bool AlmostEquals(this double self, double other)
        {
            return AlmostEquals(self, other, DefaultDoubleTolerance);
        }

        public static bool AlmostEquals(this double self, double other, double tolerance)
        {
            // wrong - don't do this!
            // A fixed tolerance chosen because it “looks small” could actually be way too large when
            // the numbers being compared are very small as well.
            //return (Math.Abs(self - other) <= tolerance);

            if (self == other)
            {
                // shortcut, handles infinities
                return true;
            }

            double diff = Math.Abs(self - other);
            if (self == 0 || other == 0 || diff < double.Epsilon)
            {
                // self or other is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < tolerance;
            }

            double selfAbs = Math.Abs(self);
            double otherAbs = Math.Abs(other);
            // use relative error
            return diff / (selfAbs + otherAbs) < tolerance;
        }
    }
}