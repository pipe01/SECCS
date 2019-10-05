using System;
using System.Reflection;

namespace FUCC
{
    internal static class Extensions
    {
        public static Type GetConcreteType(this FieldInfo field)
            => field.GetCustomAttribute<ConcreteTypeAttribute>()?.Type;
    }
}
