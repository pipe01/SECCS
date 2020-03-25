using SECCS.Internal;
using System;
using System.Runtime.CompilerServices;

namespace SECCS.Formats
{
    internal class TupleFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => type.Name.StartsWith("ValueTuple") && type.Namespace == "System";

        public object Read(Type type, IReadFormatContext<T> context)
        {
            int length = context.Read<int>("Length");

            var itemTypes = ReflectionUtils.GetGenericParams(type);
            var items = new object[length];

            for (int i = 0; i < length; i++)
            {
                items[i] = context.Read(itemTypes[i], () => $"Item{i + 1}");
            }

            return ReflectionUtils.New(type, items);
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
#if NETSTANDARD2_1 || NETCOREAPP
            var tuple = (ITuple)obj;

            context.Write(tuple.Length, "Length");

            for (int i = 0; i < tuple.Length; i++)
            {
                context.Write(tuple[i], $"Item{i + 1}");
            }
#else
            Type t = obj.GetType();
            int itemCount = t.GetGenericArguments().Length;

            context.Write(itemCount, "Length");

            for (int i = 1; i < itemCount + 1; i++)
            {
                var item = t.GetField($"Item{i}");

                context.Write(item.GetValue(obj), $"Item{i}");
            }
#endif
        }
    }
}