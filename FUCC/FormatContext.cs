using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FUCC
{
    public class FormatContext
    {
        internal IEnumerable<ITypeFormat> Formats { get; }
        public Type Type { get; }
        public Expression Buffer { get; }

        internal FormatContext(IEnumerable<ITypeFormat> formats, Type type, Expression buffer)
        {
            this.Formats = formats;
            this.Type = type;
            this.Buffer = buffer;
        }

        public ITypeFormat GetFormat<T>() => GetFormat(typeof(T));
        public ITypeFormat GetFormat(Type type) => Formats.FirstOrDefault(o => o.CanFormat(type));

        public FormatContext WithType<T>() => WithType(typeof(T));
        public FormatContext WithType(Type type) => new FormatContext(Formats, type, Buffer);
    }

    public class FormatContextWithValue : FormatContext
    {
        public Expression Value { get; }

        internal FormatContextWithValue(IEnumerable<ITypeFormat> formats, Type type, Expression buffer, Expression value)
            : base(formats, type, buffer)
        {
            this.Value = value;
        }

        public new FormatContextWithValue WithType(Type type) => new FormatContextWithValue(Formats, type, Buffer, Value);

        public FormatContextWithValue WithValue(Expression value) => new FormatContextWithValue(Formats, Type, Buffer, value);
    }
}
