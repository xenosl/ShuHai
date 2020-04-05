using System;
using System.Collections.Generic;
using System.Reflection;
using ShuHai.Reflection;

namespace ShuHai.XConverts
{
    public class XConvertMemberPriorityComparer : IComparer<AssignableMember>
    {
        public static readonly XConvertMemberPriorityComparer Instance = new XConvertMemberPriorityComparer();

        public int Compare(AssignableMember l, AssignableMember r)
        {
            if (l == r)
                return 0;
            if (l == null)
                return -1;
            if (r == null)
                return 1;

            MemberInfo lm = l.Info, rm = r.Info;
            Type lt = lm.DeclaringType, rt = rm.DeclaringType;
            var d = lt.GetDeriveDepth(rt);
            PropertyInfo lp = lm as PropertyInfo, rp = rm as PropertyInfo;
            FieldInfo lf = lm as FieldInfo, rf = rm as FieldInfo;

            if (lp == null && rp == null && lf == null && rf == null)
                return 0;

            // Property got higher priority.
            if (lp != null && rf != null)
                return 1;
            if (rp != null && lf != null)
                return -1;

            // Most derived member got higher priority
            return d;
        }
    }
}