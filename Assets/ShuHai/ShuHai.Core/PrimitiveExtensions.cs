using System;

namespace ShuHai
{
    public static class PrimitiveExtensions
    {
        public static bool AlmostEquals(this float self, float other, float epsilon = float.Epsilon)
        {
            // wrong - don't do this!
            // A fixed epsilon chosen because it “looks small” could actually be way too large when
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
                return diff < epsilon;
            }
            
            float selfAbs = Math.Abs(self);
            float otherAbs = Math.Abs(other);
            // use relative error
            return diff / (selfAbs + otherAbs) < epsilon;
        }
    }
}