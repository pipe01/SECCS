using System;

namespace SECCS.Formats.Write
{
    public class ArrayFormatWriter<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type) => type.IsArray;

        public void Write(object obj, WriteFormatContext<TWriter> context)
        {
            var arr = (Array)obj;

            context.Serialize(arr.Length, "Length");

            int i = 0;
            foreach (var item in arr)
            {
                context.Serialize(item, $"[{i++}]");
            }
        }
    }
}
