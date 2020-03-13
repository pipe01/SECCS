using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SeccsIgnoreAttribute : Attribute
    {
    }
}
