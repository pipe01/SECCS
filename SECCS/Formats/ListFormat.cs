using System;
using System.Collections;

namespace SECCS.Formats
{
    [FormatPriority(-10)]
    internal class ListFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(IList).IsAssignableFrom(type) && type.GetGenericArguments().Length > 0;

        public object Read(Type type, IReadFormatContext<T> context)
        {
            var elementType = type.GetGenericArguments()[0];

            var l = (IList)Activator.CreateInstance(type);

            int count = context.Read<int>("Count");

            for (int i = 0; i < count; i++)
            {
                var value = context.Read(elementType, $"[{i}]");
                l.Add(value);
            }

            return l;
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
