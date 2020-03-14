using System;
using System.Collections;
using System.Collections.Generic;

namespace SECCS.Formats
{
    internal class DictionaryWriteFormat<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type)
            => type.Name == typeof(IReadOnlyDictionary<,>).Name
            || type.GetInterface(typeof(IReadOnlyDictionary<,>).Name) != null;

        public void Write(object obj, WriteFormatContext<TWriter> context)
        {
            var count = (int)obj.GetType().GetProperty("Count").GetValue(obj);

            context.Write(count, "Count");

            var dic = (IDictionary)obj;

            int i = 0;
            foreach (var key in dic.Keys)
            {
                context.Write(key, $"[{i}].Key");
                context.Write(dic[key], $"[{i}].Value");

                i++;
            }
        }
    }
}
