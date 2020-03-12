using System;

namespace SECCS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormatPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public FormatPriorityAttribute(int priority)
        {
            this.Priority = priority;
        }
    }
}
