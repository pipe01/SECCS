using SECCS.Internal;
using System;
using System.Collections;

namespace SECCS.Formats
{
    [FormatPriority(-10)]
    internal class ListFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(IList).IsAssignableFrom(type) && (type.GetGenericArguments().Length > 0 || type.IsArray);

        public object Read(Type type, IReadFormatContext<T> context)
        {
            IList list;

            int count = context.Read<int>("Count");

            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                list = Array.CreateInstance(elementType, count);

                for (int i = 0; i < count; i++)
                {
                    list[i] = context.Read(elementType, () => $"[{i}]");
                }
            }
            else
            {
                list = (IList)ReflectionUtils.New(type);

                var elementType = ReflectionUtils.GetGenericParams(type)[0];

                for (int i = 0; i < count; i++)
                {
                    list.Add(context.Read(elementType, () => $"[{i}]"));
                }
            }

            return list;
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            var l = (IList)obj;

            context.Write(l.Count, "Count");

            int i = 0;
            foreach (var item in l)
            {
                context.Write(item, $"[{i++}]");
            }
        }
    }
}
