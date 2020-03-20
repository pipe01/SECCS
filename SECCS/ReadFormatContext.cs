using SECCS.Exceptions;
using System;

namespace SECCS
{
    public interface IReadFormatContext
    {
        FormatOptions Options { get; }

        object Read(Type type, string path = "<>", bool nullCheck = true);
    }

    public interface IReadFormatContext<TReader> : IReadFormatContext
    {
        TReader Reader { get; }
    }

    public readonly struct ReadFormatContext<TReader> : IReadFormatContext<TReader>
    {
        public IBufferReader<TReader> BufferReader { get; }

        public TReader Reader { get; }

        public FormatOptions Options { get; }

        internal string Path { get; }

        internal ReadFormatContext(IBufferReader<TReader> bufferReader, TReader reader, string path, FormatOptions options = null)
        {
            if (bufferReader == null)
                throw new ArgumentNullException(nameof(bufferReader));

            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            this.BufferReader = bufferReader;
            this.Reader = reader;
            this.Path = path ?? "";
            this.Options = options ?? new FormatOptions();
        }

        public object Read(Type type, string path = "<>", bool nullCheck = true)
        {
            var fullPath = $"{Path}.{path}";

            if (nullCheck && !type.IsValueType)
            {
                var nullByte = this.Read<byte>("@Null");

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
    }
}
