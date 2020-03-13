using System;

namespace SECCS
{
    public interface IReadFormat<TReader> : IFormat
    {
        object Read(Type type, ReadFormatContext<TReader> context);
    }

    public abstract class ReadFormat<T, TReader> : IReadFormat<TReader>
    {
        bool IFormat.CanFormat(Type type) => CanFormatInheritedTypes ? typeof(T).IsAssignableFrom(type) : type == typeof(T);
        object IReadFormat<TReader>.Read(Type type, ReadFormatContext<TReader> context) => Read(context);

        protected virtual bool CanFormatInheritedTypes => true;

        public abstract T Read(ReadFormatContext<TReader> context);
    }

    public delegate T ReadDelegate<T, TReader>(ReadFormatContext<TReader> reader);

    public sealed class DelegateReadFormat<T, TReader> : ReadFormat<T, TReader>
    {
        protected override bool CanFormatInheritedTypes { get; }

        private readonly ReadDelegate<T, TReader> Reader;

        public DelegateReadFormat(ReadDelegate<T, TReader> readFunc, bool canFormatInheritedTypes = true)
        {
            this.Reader = readFunc ?? throw new ArgumentNullException(nameof(readFunc));
            this.CanFormatInheritedTypes = canFormatInheritedTypes;
        }

        public override T Read(ReadFormatContext<TReader> context) => Reader(context);
    }
}
