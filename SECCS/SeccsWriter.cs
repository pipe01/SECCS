using System;

namespace SECCS
{
    public sealed class SeccsWriter<TWriter> : IBufferWriter<TWriter>
    {
        public FormatCollection<IWriteFormat<TWriter>> Formats { get; } = new FormatCollection<IWriteFormat<TWriter>>();

        public FormatOptions Options { get; }

        public SeccsWriter(FormatOptions options = null)
        {
            this.Options = options ?? new FormatOptions();

            Formats.Discover();
        }

        internal SeccsWriter(FormatCollection<IWriteFormat<TWriter>> formats, FormatOptions options = null) : this(options)
        {
            this.Formats = formats ?? throw new ArgumentNullException(nameof(formats));
        }

        public void Serialize(TWriter writer, object obj, WriteFormatContext<TWriter>? context = null)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Cannot directly serialize a null object");

            Type t = obj.GetType();

            var format = Formats.GetFor(t, Options);
            context = context ?? new WriteFormatContext<TWriter>(this, writer, t.Name);

            format.Write(obj, context.Value);
        }
    }
}
