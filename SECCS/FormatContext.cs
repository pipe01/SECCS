using System;

namespace SECCS
{
    public readonly struct WriteFormatContext<TWriter>
    {
        public IBufferWriter<TWriter> BufferWriter { get; }

        public TWriter Writer { get; }

        internal string Path { get; }

        internal WriteFormatContext(IBufferWriter<TWriter> bufferWriter, TWriter writer, string path)
        {
            this.BufferWriter = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.Path = path ?? "";
        }

        public void Serialize(object obj, string path)
        {
            BufferWriter.Serialize(Writer, obj, new WriteFormatContext<TWriter>(BufferWriter, Writer, $"{Path}.{path}"));
        }
    }
}
