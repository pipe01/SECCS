using System;
using System.Collections;
using System.Collections.Generic;

namespace SECCS.Formats
{
    internal class DictionaryWriteFormat<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type, FormatOptions options)
            => type.Name == typeof(IReadOnlyDictionary<,>).Name
            || type.GetInterface(typeof(IReadOnlyDictionary<,>).Name) != null;

        public void Write(object obj, IWriteFormatContext<TWriter> context)
        {
            var dic = (IDictionary)obj;

            context.Write(dic.Count, "Count");

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
