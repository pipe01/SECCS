using System;

namespace SECCS
{
    public interface IWriteFormat<TWriter> : IFormat
    {
        void Write(TWriter writer, object obj, WriteFormatContext<TWriter> context);
    }

    public abstract class WriteFormat<T, TWriter> : IWriteFormat<TWriter>
    {
        bool IFormat.CanFormat(Type type) => type == typeof(T);

        void IWriteFormat<TWriter>.Write(TWriter writer, object obj, WriteFormatContext<TWriter> context)
            => Write(writer, (T)obj, context);

        protected abstract void Write(TWriter writer, T obj, WriteFormatContext<TWriter> context);
    }

    public delegate void WriteDelegate<T, TWriter>(TWriter writer, T obj, WriteFormatContext<TWriter> context);

    public class DelegateWriteFormat<T, TWriter> : WriteFormat<T, TWriter>
    {
        private readonly WriteDelegate<T, TWriter> Writer;

        public DelegateWriteFormat(WriteDelegate<T, TWriter> writer)
        {
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        protected override void Write(TWriter writer, T obj, WriteFormatContext<TWriter> context) => Writer(writer, obj, context);
    }
}
