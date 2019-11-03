using System;

namespace SECCS.Attributes
{
    /// <summary>
    /// Marks this constructor as the one used for <see cref="ISeccsSerializable{TBuffer}"/>. This attribute isn't required,
    /// however it can be useful to highlight that this constructor is in fact in use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false)]
    public class SeccsConstructorAttribute : Attribute
    {
    }
}
