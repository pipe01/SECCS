using SECCS.Exceptions;
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
            if (bufferWriter == null)
                throw new ArgumentNullException(nameof(bufferWriter));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            this.BufferWriter = bufferWriter;
            this.Writer = writer;
            this.Path = path ?? "";
        }

        /// <summary>
        /// Serializes an object to the writer using the same buffer writer that invoked the operation this context represents.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="path">The path of this object</param>
        public WriteFormatContext<TWriter> Write(object obj, string path = "<>", bool nullMark = true)
        {
            var fullPath = $"{Path}.{path}";

            try
            {
                BufferWriter.Serialize(Writer, obj, new WriteFormatContext<TWriter>(BufferWriter, Writer, fullPath));
            }
            catch (Exception ex)
            {
                throw new FormattingException($"Failed to write object of type {obj.GetType()} at path {fullPath}", ex);
            }

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
            if (bufferReader == null)
                throw new ArgumentNullException(nameof(bufferReader));

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            this.BufferReader = bufferReader;
            this.Reader = reader;
            this.Path = path ?? "";
        }

        public object Read(Type type, string path = "<>", bool nullCheck = true)
        {
            var fullPath = $"{Path}.{path}";

            if (nullCheck && !type.IsValueType)
            {
                var nullByte = Read<byte>("@Null");

                if (nullByte == 0)
                {
                    return null;
                }
                else if (nullByte != 1)
                {
                    throw new FormattingException($"Invalid null marker found: {nullByte}");
                }
            }

            try
            {
                return BufferReader.Deserialize(Reader, type, new ReadFormatContext<TReader>(BufferReader, Reader, fullPath));
            }
            catch (Exception ex)
            {
                throw new FormattingException($"Failed to read type {type} at path {fullPath}", ex);
            }
        }

        public T Read<T>(string path = "<>", bool nullCheck = true) => (T)(Read(typeof(T), path, nullCheck) ?? default(T));
    }
}
