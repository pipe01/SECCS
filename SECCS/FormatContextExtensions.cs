using System;
using System.Linq.Expressions;

namespace SECCS
{
    public static class FormatContextExtensions
    {
        /// <summary>
        /// Shortcut for <c>context.GetFormat(type).Serialize(context.WithType(type).WithValue(value))</c>
        /// </summary>
        public static Expression Write(this FormatContextWithValue context, Type type, Expression value)
            => context.Formats.Get(type).Serialize(context.WithType(type).WithValue(value));

        /// <summary>
        /// Shortcut for <c>context.Write(typeof(T), value)</c>
        /// </summary>
        public static Expression Write<T>(this FormatContextWithValue context, Expression value)
            => context.Write(typeof(T), value);

        /// <summary>
        /// Shortcut for <c>context.GetFormat(type).Deserialize(context.WithType(type))</c>
        /// </summary>
        public static Expression Read(this FormatContext context, Type type, Type concreteType = null)
            => context.Formats.Get(type).Deserialize(context.WithType(type).WithConcreteType(concreteType));

        /// <summary>
        /// Shortcut for <c>context.Read(typeof(T))</c>
        /// </summary>
        public static Expression Read<T>(this FormatContext context)
            => context.Read(typeof(T));
    }
}
