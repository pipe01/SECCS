using SECCS.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SECCS
{
    public interface IWriteFormatContext<TWriter>
    {
        FormatOptions Options { get; }

        string Path { get; }

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

        public FormatOptions Options { get; }

        internal Stack<string> PathStack { get; }

        public string Path
        {
            get
            {
#if NETFRAMEWORK || NETSTANDARD2_0
                return string.Join(".", PathStack.Reverse());
#else
                return string.Join('.', PathStack.Reverse());
#endif
            }
        }

        internal WriteFormatContext(IBufferWriter<TWriter> bufferWriter, TWriter writer, string path, FormatOptions options = null) : this(bufferWriter, writer, new Stack<string>(), options)
        {
            PathStack.Push(path);
        }
        
        internal WriteFormatContext(IBufferWriter<TWriter> bufferWriter, TWriter writer, Stack<string> pathStack, FormatOptions options = null)
        {
            if (bufferWriter == null)
                throw new ArgumentNullException(nameof(bufferWriter));

            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            this.BufferWriter = bufferWriter;
            this.Writer = writer;
            this.Options = options ?? new FormatOptions();
            this.PathStack = pathStack ?? new Stack<string>();
        }

        /// <inheritdoc/>
        public IWriteFormatContext<TWriter> Write(object obj, string path = "<>", bool nullMark = true)
        {
            if (nullMark && !(obj is ValueType))
                Write((byte)(obj == null ? 0 : 1), "@Null");

            if (obj != null)
            {
                PathStack.Push(path);

                try
                {
                    BufferWriter.Serialize(Writer, obj, new WriteFormatContext<TWriter>(BufferWriter, Writer, this.PathStack));
                }
                catch (Exception ex)
                {
                    throw new FormattingException($"Failed to write object of type {obj.GetType()} at path {Path}", ex);
                }
                finally
                {
                    PathStack.Pop();
                }
            }

            return this;
        }
    }
}
