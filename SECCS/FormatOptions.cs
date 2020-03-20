using System;
using System.Collections.Generic;

namespace SECCS
{
    public sealed class FormatOptions
    {
        public ICollection<Type> ExcludedFromPrimitive { get; } = new HashSet<Type>();
    }
}
