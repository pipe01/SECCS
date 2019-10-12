using System;

namespace SECCS.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SeccsIgnoreAttribute : Attribute
    {
    }
}
