using System;

namespace FUCC
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ReadMethodAttribute : Attribute
    {
        internal Type ForType { get; }

        public ReadMethodAttribute()
        {
        }

        public ReadMethodAttribute(Type forType)
        {
            this.ForType = forType;
        }
    }
}
