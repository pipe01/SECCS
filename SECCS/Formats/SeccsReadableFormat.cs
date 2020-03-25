using SECCS.Interfaces;
using SECCS.Internal;
using System;

namespace SECCS.Formats
{
    internal class SeccsReadableFormat<TReader> : IReadFormat<TReader>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(ISeccsReadable<TReader>).IsAssignableFrom(type);

        public object Read(Type type, IReadFormatContext<TReader> context)
        {
            var obj = (ISeccsReadable<TReader>)ReflectionUtils.New(type);

            obj.Read(context.Reader);

            return obj;
        }
    }
}
