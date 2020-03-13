using System;

namespace SECCS.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public sealed class SeccsConstructorAttribute : Attribute
    {
    }
}
