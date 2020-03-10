using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SECCS.DefaultFormats
{
    using static Expression;

    internal class KeyValuePairFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);

        public Expression Deserialize(FormatContext context)
        {
            var genericArgs = context.Type.GetGenericArguments();

            //new KeyValuePair<TKey, TValue>(Read(TKey), Read(TValue))
            return New(context.Type.GetConstructor(genericArgs),
                context.Read(genericArgs[0], reason: "kvp k"),
                context.Read(genericArgs[1], reason: "kvp v"));
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var genericArgs = context.Type.GetGenericArguments();

            //Write(value.Key);
            //Write(value.Value);
            return Block(
                context.Write("Key", genericArgs[0], Property(context.Value, "Key")),
                context.Write("Value", genericArgs[1], Property(context.Value, "Value")));
        }
    }
}
