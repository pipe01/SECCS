using SECCS.Exceptions;
using SECCS.Formats.Read;
using SECCS.Formats.Write;
using System;

namespace SECCS
{
    public sealed class SeccsWriter<TWriter> : IBufferWriter<TWriter>
    {
        public FormatCollection<IWriteFormat<TWriter>> WriteFormats { get; } = new FormatCollection<IWriteFormat<TWriter>>();

        public SeccsWriter()
        {
            WriteFormats.Discover();
        }

        public void Serialize(TWriter writer, object obj, WriteFormatContext<TWriter>? context = null)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Cannot directly serialize a null object");

            Type t = obj.GetType();

            var format = WriteFormats.GetFor(t);
            if (format == null)
                throw new FormatNotFoundException(t);

            context ??= new WriteFormatContext<TWriter>(this, writer, "");

            format.Write(obj, context.Value);
        }
    }
}
