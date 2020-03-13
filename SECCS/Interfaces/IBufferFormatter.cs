using System;
using System.Collections.Generic;

namespace SECCS
{
    public interface IBufferReader<TReader>
    {
        FormatCollection<IReadFormat<TReader>> Formats { get; }

        object Deserialize(TReader reader, Type objType, ReadFormatContext<TReader>? context = null);
        T Deserialize<T>(TReader reader, ReadFormatContext<TReader>? context = null);
    }

    public interface IBufferWriter<TWriter>
    {
        FormatCollection<IWriteFormat<TWriter>> Formats { get; }

        void Serialize(TWriter writer, object obj, WriteFormatContext<TWriter>? context = null);
    }
}
