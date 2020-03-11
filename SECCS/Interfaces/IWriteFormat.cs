using System;

namespace SECCS
{
    public interface IWriteFormat<TWriter> : IFormat
    {
        void Write(object obj, WriteFormatContext<TWriter> context);
    }

    public abstract class WriteFormat<T, TWriter> : IWriteFormat<TWriter>
    {
        bool IFormat.CanFormat(Type type) => type == typeof(T);

        void IWriteFormat<TWriter>.Write(object obj, WriteFormatContext<TWriter> context)
            => Write((T)obj, context);

        protected abstract void Write(T obj, WriteFormatContext<TWriter> context);
    }

    public delegate void WriteDelegate<T, TWriter>(T obj, WriteFormatContext<TWriter> context);

    public class DelegateWriteFormat<T, TWriter> : WriteFormat<T, TWriter>
    {
        private readonly WriteDelegate<T, TWriter> Writer;

        public DelegateWriteFormat(WriteDelegate<T, TWriter> writer)
        {
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        protected override void Write(T obj, WriteFormatContext<TWriter> context) => Writer(obj, context);
    }
}
