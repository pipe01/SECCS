using System;

namespace SECCS
{
    public interface IWriteFormat<TWriter> : IFormat
    {
        void Write(object obj, IWriteFormatContext<TWriter> context);
    }

    public abstract class WriteFormat<T, TWriter> : IWriteFormat<TWriter>
    {
        bool IFormat.CanFormat(Type type, FormatOptions options) => CanFormatInheritedTypes ? typeof(T).IsAssignableFrom(type) : type == typeof(T);
        void IWriteFormat<TWriter>.Write(object obj, IWriteFormatContext<TWriter> context) => Write((T)obj, context);

        protected virtual bool CanFormatInheritedTypes => true;

        protected abstract void Write(T obj, IWriteFormatContext<TWriter> context);
    }

    public delegate void WriteDelegate<T, TWriter>(T obj, IWriteFormatContext<TWriter> context);

    public class DelegateWriteFormat<T, TWriter> : WriteFormat<T, TWriter>
    {
        protected override bool CanFormatInheritedTypes { get; }

        private readonly WriteDelegate<T, TWriter> Writer;

        public DelegateWriteFormat(WriteDelegate<T, TWriter> writer, bool canFormatInheritedTypes = true)
        {
            this.Writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.CanFormatInheritedTypes = canFormatInheritedTypes;
        }

        protected override void Write(T obj, IWriteFormatContext<TWriter> context) => Writer(obj, context);
    }
}
