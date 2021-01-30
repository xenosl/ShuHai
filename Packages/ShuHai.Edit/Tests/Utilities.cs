using System;
using System.Collections;

namespace ShuHai.Edit.Tests
{
    internal class Utilities
    {
        public static bool CollectionEquals(IEnumerable a, IEnumerable b)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));
            if (b == null)
                throw new ArgumentNullException(nameof(b));

            if (a.GetType() != b.GetType())
                return false;

            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            while (ea.MoveNext())
            {
                if (!eb.MoveNext())
                    return false;

                object ia = ea.Current, ib = eb.Current;
                if (ia is IEnumerable iae && ib is IEnumerable ibe)
                {
                    if (!CollectionEquals(iae, ibe))
                        return false;
                }
                else
                {
                    if (!Equals(ea.Current, eb.Current))
                        return false;
                }
            }
            return true;
        }
    }
}