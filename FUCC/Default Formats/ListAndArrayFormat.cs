using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FUCC.DefaultFormats
{
    using static Expression;

    internal class ListAndArrayFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => type.IsArray || (typeof(IList).IsAssignableFrom(type) && type.IsGenericType);

        public Expression Deserialize(FormatContext context)
        {
            var t = context.Type;
            var arrType = t.IsArray ? t.GetElementType() : t.GetGenericArguments()[0];
            var indexVar = Variable(typeof(int), "_idx");

            var arrLengthVar = Variable(typeof(int), "_len");
            var resultVar = Variable(t, "_ret"); //t is an array type
            var breakLabel = Label("_break");

            //i = 0;
            //length = msg.ReadInt32();
            //result = new T[length];
            //
            //while (true)
            //{
            //    if (index < length)
            //    {
            //        result[i] = Read(T);
            //        i++;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            return Block(new[] { indexVar, arrLengthVar, resultVar },
                Assign(indexVar, Constant(0)),

                Assign(arrLengthVar, context.Read<int>()),

                Assign(resultVar, t.IsArray ? (Expression)NewArrayBounds(arrType, arrLengthVar) : New(t)),

                Loop(IfThenElse(
                    LessThan(indexVar, arrLengthVar),
                    Block(
                        t.IsArray
                            ? (Expression)Assign(ArrayAccess(resultVar, indexVar), context.Read(arrType))
                            : Call(resultVar, "Add", null, context.Read(arrType)),
                        PostIncrementAssign(indexVar)),
                    Break(breakLabel)), breakLabel),
                resultVar);
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var t = context.Type;

            var itemType = t.IsArray ? t.GetElementType() : t.GetGenericArguments()[0];
            var indexVar = Variable(typeof(int), "_idx");

            var arrLength = Property(context.Value, t.IsArray ? "Length" : "Count");
            var breakLabel = Label("_break");

            var block = new List<Expression>
            {
                //msg.Write(length);
                context.GetFormat(typeof(int)).Serialize(context.WithType(typeof(int)).WithValue(arrLength)),

                //i = 0;
                Assign(indexVar, Constant(0)),

                //while (true)
                //{
                //    if (index < length)
                //    {
                //        Write(value[i]);
                //        i++;
                //    }
                //    else
                //    {
                //        break;
                //    }
                //}
                Loop(IfThenElse(
                    LessThan(indexVar, arrLength),
                    Block(
                        context.Write(itemType, t.IsArray
                                    ? (Expression)ArrayAccess(context.Value, indexVar)
                                    : Call(context.Value, "get_Item", null, indexVar)),
                        PostIncrementAssign(indexVar)),
                    Break(breakLabel)), breakLabel)
            };

            return Block(new[] { indexVar }, block);
        }
    }
}
