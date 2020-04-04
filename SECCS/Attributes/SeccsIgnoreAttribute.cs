using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class SeccsIgnoreAttribute : Attribute
    {
    }
}
