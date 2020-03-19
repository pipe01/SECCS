using System.Reflection;

namespace SECCS.Internal
{
    internal static class MemberInfoExtensions
    {
        public static bool HasSeccsMember(this MemberInfo member)
            => member.IsDefined(typeof(SeccsMemberAttribute));
    }
}
