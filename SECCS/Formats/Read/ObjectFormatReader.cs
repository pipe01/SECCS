using System;

namespace SECCS.Formats.Read
{
    public sealed class ObjectFormatReader<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type) => !type.IsPrimitive;

        public object Read(Type type, ReadFormatContext<TReader> context)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var item in type.GetProperties())
            {
                var value = context.Deserialize(item.PropertyType, item.Name);
                item.SetValue(obj, value);
            }

            return obj;
        }
    }
}
