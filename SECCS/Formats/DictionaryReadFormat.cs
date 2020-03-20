using System;
using System.Collections;

namespace SECCS.Formats
{
    internal class DictionaryReadFormat<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type, FormatOptions options) => type.IsGenericType && !type.IsInterface && typeof(IDictionary).IsAssignableFrom(type);

        public object Read(Type type, IReadFormatContext<TReader> context)
        {
            var dic = (IDictionary)Activator.CreateInstance(type);

            var genericArgs = type.GetGenericArguments();
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];

            int count = context.Read<int>("Count");

            for (int i = 0; i < count; i++)
            {
                var key = context.Read(keyType, $"[{i}].Key");
                dic[key] = context.Read(valueType, $"[{i}].Value");
            }

            return dic;
        }
    }
}
