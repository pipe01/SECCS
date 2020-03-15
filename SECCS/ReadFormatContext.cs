using SECCS.Exceptions;
using System;

namespace SECCS
{
    public interface IReadFormatContext<TReader>
    {
        TReader Reader { get; }

        object Read(Type type, string path = "<>", bool nullCheck = true);
        T Read<T>(string path = "<>", bool nullCheck = true);
    }

    public readonly struct ReadFormatContext<TReader> : IReadFormatContext<TReader>
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
