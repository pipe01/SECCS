using System;

namespace SECCS.Exceptions
{
    /// <summary>
    /// Thrown when a type is serialized or deserialized and a format can't be found
    /// for it.
    /// </summary>
    public class FormatNotFoundException : Exception
    {
        public Type ObjectType { get; }

        public FormatNotFoundException(Type objType) : base($"A format could not be found for type {objType}")
        {
            this.ObjectType = objType;
        }
    }
}
