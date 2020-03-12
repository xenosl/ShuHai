using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.SConverts
{
    public class SConverterPriorityComparer<T> : IComparer<T>
        where T : ISConverter
    {
        public static readonly SConverterPriorityComparer<T> Instance = new SConverterPriorityComparer<T>();

        public int Compare(T l, T r)
        {
            if (l == null && r == null)
                return 0;
            if (l == null)
                return -1;
            if (r == null)
                return 1;

            Type ltt = l.TargetType, rtt = r.TargetType;
            if (ltt == rtt)
            {
                int lp = PriorityOf(l.GetType()), rp = PriorityOf(r.GetType());
                return lp.CompareTo(rp);
            }

            if (ltt.IsAssignableFrom(rtt))
                return -1;
            if (rtt.IsAssignableFrom(ltt))
                return 1;

            return 0;
        }

        private static int PriorityOf(Type type)
        {
            var a = type.GetCustomAttribute<SConverterAttribute>();
            return a?.Priority ?? 0;
        }

        private SConverterPriorityComparer() { }
    }
}