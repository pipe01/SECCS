using SECCS.Internal;
using System;
using System.Linq.Expressions;

namespace SECCS
{
    public static class FormatContextExtensions
    {
        internal static Expression Write(this FormatContextWithValue context, string reason, Type type, Expression value)
        {
            var ctx = context.WithType(type).WithReason(context.Reason + "." + reason).WithValue(value);
            var expr = context.Formats.Get(type).Serialize(ctx);

            return context.Options.DebugSerialize ? expr.Wrapped(ctx) : expr;
        }

        /// <summary>
        /// Shortcut for <c>context.GetFormat(type).Serialize(context.WithType(type).WithValue(value))</c>
        /// </summary>
        public static Expression Write(this FormatContextWithValue context, Type type, Expression value)
            => context.Write(null, type, value);


        internal static Expression Write<T>(this FormatContextWithValue context, string reason, Expression value)
            => context.Write(reason, typeof(T), value);

        /// <summary>
        /// Shortcut for <c>context.Write(typeof(T), value)</c>
        /// </summary>
        public static Expression Write<T>(this FormatContextWithValue context, Expression value)
            => context.Write<T>(value);


        /// <summary>
        /// Shortcut for <c>context.GetFormat(type).Deserialize(context.WithType(type))</c>
        /// </summary>
        public static Expression Read(this FormatContext context, Type type, Type concreteType = null, string reason = null)
            => context.Formats.Get(type).Deserialize(context.WithType(type).WithConcreteType(concreteType).WithReason(reason));

        /// <summary>
        /// Shortcut for <c>context.Read(typeof(T))</c>
        /// </summary>
        public static Expression Read<T>(this FormatContext context, string reason = null)
            => context.Read(typeof(T), reason: reason);
    }
}
