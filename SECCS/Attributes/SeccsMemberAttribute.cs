using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class SeccsMemberAttribute : Attribute
    {
    }
}
