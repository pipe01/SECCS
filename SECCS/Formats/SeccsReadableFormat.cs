using SECCS.Interfaces;
using System;

namespace SECCS.Formats
{
    internal class SeccsReadableFormat<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(ISeccsReadable<TReader>).IsAssignableFrom(type);

        public object Read(Type type, IReadFormatContext<TReader> context)
        {
            var obj = Activator.CreateInstance(type);
            var readable = (ISeccsReadable<TReader>)obj;

            readable.Read(context.Reader);

            return obj;
        }
    }
}
