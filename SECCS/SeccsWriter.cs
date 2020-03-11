using SECCS.Exceptions;
using System;

namespace SECCS
{
    public sealed class SeccsWriter<TWriter> : IBufferWriter<TWriter>
    {
        public FormatCollection<IWriteFormat<TWriter>> WriteFormats { get; } = new FormatCollection<IWriteFormat<TWriter>>();

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

            format.Write(writer, obj, context.Value);
        }
    }
}
