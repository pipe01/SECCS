using SECCS.Internal;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.DefaultFormats
{
    using static Expression;

    [Priority(-1)]
    internal class CollectionFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.Name == "ICollection`1" || type.GetInterface("ICollection`1") != null
                                         || type.Name == "IReadOnlyCollection`1" || type.GetInterface("IReadOnlyCollection`1") != null;

        public Expression Deserialize(FormatContext context)
        {
            var collection = context.Type.GetInterface("ICollection`1") ?? context.Type.GetInterface("IReadOnlyCollection`1") ?? context.Type;
            var itemType = collection.GetGenericArguments()[0];

            var countVar = Variable(typeof(int), "_count");
            var resultVar = Variable(context.Type, "_ret");

            var indexVar = Variable(typeof(int), "_idx");
            var breakLabel = Label("_break");

            var addMethod = context.DeserializableType.GetMethodInAnyInterface("Add", new[] { itemType });

            if (addMethod == null)
                throw new Exception($"Type {context.DeserializableType.FullName} doesn't have an appropiate 'Add' method");

            return Block(new[] { countVar, resultVar, indexVar },
                Assign(indexVar, Constant(0)),
                Assign(countVar, context.Read<int>("coll length")),
                Assign(resultVar, New(context.DeserializableType)),

                Loop(IfThenElse(
                    LessThan(indexVar, countVar),
                    Block(
                        Call(Convert(resultVar, addMethod.DeclaringType), addMethod, context.Read(itemType, reason: "coll item")),
                        PostIncrementAssign(indexVar)),
                    Break(breakLabel)), breakLabel),

                resultVar
            );
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var collection = context.Type.GetInterface("ICollection`1") ?? context.Type.GetInterface("IReadOnlyCollection`1") ?? context.Type;
            var itemType = collection.GetGenericArguments()[0];
            var enumerator = Variable(typeof(IEnumerator));
            var breakLabel = Label("_break");

            return Block(new[] { enumerator },
                Assign(enumerator, Call(Convert(context.Value, typeof(IEnumerable)), "GetEnumerator", null)),

                context.Write<int>("length", Property(context.Value, collection.GetProperty("Count"))),

                Loop(IfThenElse(
                        IsTrue(Call(enumerator, "MoveNext", null)),
                        context.Write("list item", itemType, Convert(Property(enumerator, "Current"), itemType)),
                        Break(breakLabel)
                ), breakLabel));
        }
    }
}
