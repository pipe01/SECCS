using System;

namespace FUCC.Internal
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class PriorityAttribute : Attribute
    {
        public int Priority { get; }

        public PriorityAttribute(int priority)
        {
            this.Priority = priority;
        }
    }
}
