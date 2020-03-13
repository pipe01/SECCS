using SECCS.Interfaces;
using System;

namespace SECCS.Formats
{
    internal class SeccsWriteableFormat<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type) => typeof(ISeccsWriteable<TWriter>).IsAssignableFrom(type);

        public void Write(object obj, WriteFormatContext<TWriter> context)
        {
            var writeable = (ISeccsWriteable<TWriter>)obj;

            writeable.Write(context.Writer);
        }
    }
}
