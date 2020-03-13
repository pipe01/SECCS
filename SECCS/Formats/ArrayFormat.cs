using System;

namespace SECCS.Formats
{
    [FormatPriority(-10)]
    public class ArrayFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type) => type.IsArray;

        public object Read(Type type, ReadFormatContext<T> context)
        {
            var itemType = type.GetElementType();

            int length = context.Read<int>("Length");
            var arr = (Array)Activator.CreateInstance(type, length);

            for (int i = 0; i < length; i++)
            {
                var value = context.Read(itemType, $"[{i}]");
                arr.SetValue(value, i);
            }

            return arr;
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            var arr = (Array)obj;

            context.Write(arr.Length, "Length");

            int i = 0;
            foreach (var item in arr)
            {
                context.Write(item, $"[{i++}]");
            }
        }
    }
}
