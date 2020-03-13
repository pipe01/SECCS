using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class SeccsConstructorAttribute : Attribute
    {
    }
}
