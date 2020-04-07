using SECCS.Internal;
using System;

namespace SECCS.Formats
{
    internal class KeyValuePairFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => type.Name == "KeyValuePair`2";

        public object Read(Type type, IReadFormatContext<T> context)
        {
            var args = ReflectionUtils.GetGenericParams(type);
            var keyType = args[0];
            var valueType = args[1];

            var key = context.Read(keyType, "Key");
            var value = context.Read(valueType, "Value");

            return ReflectionUtils.New(type, new[] { keyType, valueType }, new[] { key, value });
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            Type t = obj.GetType();

            var key = t.GetProperty("Key").GetValue(obj);
            var value = t.GetProperty("Value").GetValue(obj);

            context.Write(key, "Key");
            context.Write(value, "Value");
        }
    }
}
