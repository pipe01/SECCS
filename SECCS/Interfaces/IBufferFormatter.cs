using System;
using System.Collections.Generic;

namespace SECCS
{
    public interface IBufferReader<TReader>
    {
        FormatCollection<IReadFormat<TReader>> ReadFormats { get; }

        object Deserialize(TReader reader, Type objType, ReadFormatContext<TReader>? context = null);
        T Deserialize<T>(TReader reader, ReadFormatContext<TReader>? context = null);
    }

    public interface IBufferWriter<TWriter>
    {
        FormatCollection<IWriteFormat<TWriter>> WriteFormats { get; }

        void Serialize(TWriter writer, object obj, WriteFormatContext<TWriter>? context = null);
    }

    public interface IBufferFormatter<TBuffer> : IBufferReader<TBuffer>, IBufferWriter<TBuffer>
    {
    }
}
