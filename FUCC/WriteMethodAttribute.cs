using System;

namespace FUCC
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class WriteMethodAttribute : Attribute
    {
        internal Type ForType { get; }

        public WriteMethodAttribute()
        {
        }

        public WriteMethodAttribute(Type forType)
        {
            this.ForType = forType;
        }
    }
}
