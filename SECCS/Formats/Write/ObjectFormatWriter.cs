using System;

namespace SECCS.Formats.Write
{
    public sealed class ObjectFormatWriter<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type) => true;

        public void Write(TWriter writer, object obj, WriteFormatContext<TWriter> context)
        {
            Type t = obj.GetType();

            foreach (var item in t.GetProperties())
            {
                if (item.CanRead)
                    context.Serialize(item.GetValue(obj), item.Name);
            }
        }
    }
}
