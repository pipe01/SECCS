using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConcreteTypeAttribute : Attribute
    {
        public Type Type { get; }

        public ConcreteTypeAttribute(Type type)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }
}
