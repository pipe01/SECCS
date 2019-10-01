using System;
using System.Collections;
using System.Linq.Expressions;

namespace FUCC.DefaultFormats
{
    using static Expression;

    [Priority(-1)]
    internal class CollectionFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.GetInterface("ICollection`1") != null;

        public Expression Deserialize(FormatContext context)
        {
            var collection = context.Type.GetInterface("ICollection`1");
            var itemType = collection.GetGenericArguments()[0];

            var countVar = Variable(typeof(int), "_count");
            var resultVar = Variable(context.Type, "_ret");

            var indexVar = Variable(typeof(int), "_idx");
            var breakLabel = Label("_break");

            return Block(new[] { countVar, resultVar, indexVar },
                Assign(indexVar, Constant(0)),
                Assign(countVar, context.Read<int>()),
                Assign(resultVar, New(context.Type)),

                Loop(IfThenElse(
                    LessThan(indexVar, countVar),
                    Block(
                        Call(Convert(resultVar, collection), "Add", null, context.Read(itemType)),
                        PostIncrementAssign(indexVar)),
                    Break(breakLabel)), breakLabel),

                resultVar
            );
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var itemType = context.Type.GetInterface("ICollection`1").GetGenericArguments()[0];
            var enumerator = Variable(typeof(IEnumerator));
            var breakLabel = Label("_break");

            return Block(new[] { enumerator },
                Assign(enumerator, Call(Convert(context.Value, typeof(IEnumerable)), "GetEnumerator", null)),

                context.Write<int>(Property(context.Value, "Count")),

                Loop(IfThenElse(
                        IsTrue(Call(enumerator, "MoveNext", null)),
                        context.Write(itemType, Convert(Property(enumerator, "Current"), itemType)),
                        Break(breakLabel)
                ), breakLabel));
        }
    }
}
