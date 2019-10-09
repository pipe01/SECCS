using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadMethodAttribute : Attribute
    {
        internal Type ForType { get; }

        /// <summary>
        /// Explicitly marks this method to be used as a deserializer when calling <see cref="TypeFormat.GetFromReadAndWrite{TBuffer}"/>.
        /// </summary>
        public ReadMethodAttribute()
        {
        }

        /// <summary>
        /// Explicitly marks this method to be used as a deserializer for <paramref name="forType"/> when calling <see cref="TypeFormat.GetFromReadAndWrite{TBuffer}"/>.
        /// </summary>
        /// <param name="forType">The type that this method reads</param>
        public ReadMethodAttribute(Type forType)
        {
            this.ForType = forType;
        }
    }
}
