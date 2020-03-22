using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShuHai.XConverts
{
    public class MemberPriorityComparer : IComparer<MemberInfo>
    {
        public static readonly MemberPriorityComparer Instance = new MemberPriorityComparer();

        public int Compare(MemberInfo x, MemberInfo y)
        {
            if (x == y)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            Type tx = x.DeclaringType, ty = y.DeclaringType;
            var d = tx.GetDeriveDepth(ty);
            PropertyInfo px = x as PropertyInfo, py = y as PropertyInfo;
            FieldInfo fx = x as FieldInfo, fy = y as FieldInfo;

            if (px == null && py == null && fx == null && fy == null)
                return 0;

            // Property got higher priority.
            if (px != null && fy != null)
                return 1;
            if (py != null && fx != null)
                return -1;

            // Most derived member got higher priority
            return d;
        }
    }
}