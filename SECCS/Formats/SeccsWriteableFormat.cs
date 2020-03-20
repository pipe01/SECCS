using SECCS.Interfaces;
using System;

namespace SECCS.Formats
{
    internal class SeccsWriteableFormat<TWriter> : IWriteFormat<TWriter>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(ISeccsWriteable<TWriter>).IsAssignableFrom(type);

        public void Write(object obj, IWriteFormatContext<TWriter> context)
        {
            var writeable = (ISeccsWriteable<TWriter>)obj;

            writeable.Write(context.Writer);
        }
    }
}
