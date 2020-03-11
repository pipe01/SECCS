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

    public readonly struct ReadFormatContext<TReader>
    {
        public IBufferReader<TReader> BufferReader { get; }

        public TReader Reader { get; }

        internal string Path { get; }

        internal ReadFormatContext(IBufferReader<TReader> bufferReader, TReader reader, string path)
        {
            this.BufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
            this.Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.Path = path ?? "";
        }

        public object Deserialize(Type type, string path) => BufferReader.Deserialize(Reader, type, new ReadFormatContext<TReader>(BufferReader, Reader, $"{Path}.{path}"));

        public T Deserialize<T>(string path) => (T)Deserialize(typeof(T), path);
    }
}
