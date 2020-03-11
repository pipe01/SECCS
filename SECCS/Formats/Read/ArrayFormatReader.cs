using System;

namespace SECCS.Formats.Read
{
    public class ArrayFormatReader<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type) => type.IsArray;

        public object Read(TReader reader, Type type, ReadFormatContext<TReader> context)
        {
            var itemType = type.GetElementType();

            int length = context.Deserialize<int>("Length");
            var arr = (Array)Activator.CreateInstance(type, length);

            for (int i = 0; i < length; i++)
            {
                var value = context.Deserialize(itemType, $"[{i}]");
                arr.SetValue(value, i);
            }

            return arr;
        }
    }
}
