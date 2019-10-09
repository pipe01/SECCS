using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WriteMethodAttribute : Attribute
    {
        internal Type ForType { get; }

        /// <summary>
        /// Explicitly marks this method to be used as a serializer when calling <see cref="TypeFormat.GetFromReadAndWrite{TBuffer}"/>.
        /// </summary>
        public WriteMethodAttribute()
        {
        }

        /// <summary>
        /// Explicitly marks this method to be used as a serializer for <paramref name="forType"/> when calling <see cref="TypeFormat.GetFromReadAndWrite{TBuffer}"/>.
        /// </summary>
        /// <param name="forType">The type that this method writes</param>
        public WriteMethodAttribute(Type forType)
        {
            this.ForType = forType;
        }
    }
}
