using SECCS.Internal;
using System;
using System.Collections;

namespace SECCS.Formats
{
    internal class DictionaryReadFormat<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type, FormatOptions options) => type.IsGenericType && !type.IsInterface && typeof(IDictionary).IsAssignableFrom(type);

        public object Read(Type type, IReadFormatContext<TReader> context)
        {
            var dic = (IDictionary)ReflectionUtils.New(type);

            var genericArgs = ReflectionUtils.GetGenericParams(type);
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];

            int count = context.Read<int>("Count");

            for (int i = 0; i < count; i++)
            {
                var key = context.Read(keyType, PathGetter.Index(i));
                dic[key] = context.Read(valueType, PathGetter.Index(i));
            }

            return dic;
        }
    }
}
