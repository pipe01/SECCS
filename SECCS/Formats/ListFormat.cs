using System;
using System.Collections;
using System.Collections.Generic;

namespace SECCS.Formats
{
    [FormatPriority(-1)]
    public class ListFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type) => typeof(IList).IsAssignableFrom(type) && type.GetGenericArguments().Length > 0;

        public object Read(Type type, ReadFormatContext<T> context)
        {
            var elementType = type.GetGenericArguments()[0];

            var l = (IList)Activator.CreateInstance(type);

            int count = context.Deserialize<int>("Count");

            for (int i = 0; i < count; i++)
            {
                var value = context.Deserialize(elementType, $"[{i}]");
                l.Add(value);
            }

            return l;
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            var l = (IList)obj;

            context.Serialize(l.Count, "Count");

            int i = 0;
            foreach (var item in l)
            {
                context.Serialize(item, $"[{i++}]");
            }
        }
    }
}
