using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FUCC.DefaultFormats
{
    using static Expression;

    internal class ValueTupleFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("ValueTuple`");

        public Expression Deserialize(FormatContext context)
        {
            var genericArgs = context.Type.GetGenericArguments();

            //new ValueTuple<T1, T2, ..., T3>(Read(T1), Read(T2), ..., Read(TN))
            return New(context.Type.GetConstructor(genericArgs), genericArgs.Select(o => context.Read(o)));
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var block = new List<Expression>();

            int i = 1;
            //Write(value.Item1);
            //Write(value.Item2);
            //...
            //Write(value.ItemN);
            foreach (var item in context.Type.GetGenericArguments())
            {
                block.Add(context.Write(item, Field(context.Value, "Item" + i++)));
            }

            return Block(block);
        }
    }
}
