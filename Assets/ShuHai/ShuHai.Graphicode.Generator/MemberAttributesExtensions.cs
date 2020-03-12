using System.CodeDom;

namespace ShuHai.Graphicode.Generator
{
    public static class MemberAttributesExtensions
    {
        public static MemberAttributes SetAccessFlag(this MemberAttributes attributes, MemberAttributes flag)
        {
            return SetFlag(attributes, MemberAttributes.AccessMask, flag);
        }

        public static MemberAttributes SetScopeFlag(this MemberAttributes attributes, MemberAttributes flag)
        {
            return SetFlag(attributes, MemberAttributes.ScopeMask, flag);
        }

        public static MemberAttributes SetFlag(
            this MemberAttributes attributes, MemberAttributes mask, MemberAttributes flag)
        {
            // See MemberAttributes documentation for detailed information about setting the flag.
            return (attributes & ~mask) | flag;
        }
    }
}