using System;

namespace FUCC
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PriorityAttribute : Attribute
    {
        public int Priority { get; }

        /// <summary>
        /// Marks this type format with the given priority. Type formats with a higher priority will be used first.
        /// </summary>
        public PriorityAttribute(int priority)
        {
            this.Priority = priority;
        }
    }
}
