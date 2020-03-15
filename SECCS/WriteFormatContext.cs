using SECCS.Exceptions;
using System;

namespace SECCS
{
    public interface IWriteFormatContext<TWriter>
    {
        /// <summary>
        /// The low-level writer.
        /// </summary>
        TWriter Writer { get; }

        /// <summary>
        /// Serializes an object to the writer using the same buffer writer that invoked the operation this context represents.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="path">The path of this object</param>
        IWriteFormatContext<TWriter> Write(object obj, string path = "<>", bool nullMark = true);
    }

    /// <summary>
    /// Represents a write format's context information.
    /// </summary>
    /// <typeparam name="TWriter">The type of the writer</typeparam>
    public readonly struct WriteFormatContext<TWriter> : IWriteFormatContext<TWriter>
    {
        /// <summary>
        /// The buffer writer that invoked this operation.
        /// </summary>
        public IBufferWriter<TWriter> BufferWriter { get; }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public IWriteFormatContext<TWriter> Write(object obj, string path = "<>", bool nullMark = true)
        {
            var fullPath = $"{Path}.{path}";

            if (nullMark && !(obj is ValueType))
                Write((byte)(obj == null ? 0 : 1), "@Null");

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
}
