using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConcreteTypeAttribute : Attribute
    {
        public Type Type { get; }

        /// <summary>
        /// When applied to a field whose type is an interface, this attribute specifies the type to instantiate when
        /// deserializing. <br/>
        /// <paramref name="type"/> must of course implement the field's type.
        /// </summary>
        public ConcreteTypeAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
