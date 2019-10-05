using FUCC.Internal;
using System;
using System.Reflection;

namespace FUCC
{
    internal static class Extensions
    {
        public static Type GetConcreteType(this ClassMember field)
            => field.Member.GetCustomAttribute<ConcreteTypeAttribute>()?.Type;
    }
}
