using System;

namespace SECCS
{
    /// <summary>
    /// Represents a write format's context information.
    /// </summary>
    /// <typeparam name="TWriter">The type of the writer</typeparam>
    public readonly struct WriteFormatContext<TWriter>
    {
        /// <summary>
        /// The buffer writer that invoked this operation.
        /// </summary>
        public IBufferWriter<TWriter> BufferWriter { get; }

        /// <summary>
        /// The low-level writer.
        /// </summary>
        public TWriter Writer { get; }

        internal string Path { get; }

        internal WriteFormatContext(IBufferWriter<TWriter> bufferWriter, TWriter writer, string path)
        {
            this.BufferWriter = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.Path = path ?? "";
        }

        /// <summary>
        /// Serializes an object to the writer using the same buffer writer that invoked the operation this context represents.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="path">The path of this object</param>
        public WriteFormatContext<TWriter> Serialize(object obj, string path = "<>")
        {
            BufferWriter.Serialize(Writer, obj, new WriteFormatContext<TWriter>(BufferWriter, Writer, $"{Path}.{path}"));
            return this;
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

        public object Deserialize(Type type, string path = "<>") => BufferReader.Deserialize(Reader, type, new ReadFormatContext<TReader>(BufferReader, Reader, $"{Path}.{path}"));

        public T Deserialize<T>(string path) => (T)Deserialize(typeof(T), path);
    }
}
