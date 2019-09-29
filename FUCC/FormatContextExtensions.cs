using System;
using System.Linq.Expressions;

namespace FUCC
{
    public static class FormatContextExtensions
    {
        public static Expression Write(this FormatContextWithValue context, Type type, Expression value)
            => context.GetFormat(type).Serialize(context.WithType(type).WithValue(value));

        public static Expression Write<T>(this FormatContextWithValue context, Expression value)
            => context.Write(typeof(T), value);

        public static Expression Read(this FormatContext context, Type type)
            => context.GetFormat(type).Deserialize(context.WithType(type));

        public static Expression Read<T>(this FormatContext context)
            => context.Read(typeof(T));
    }
}
