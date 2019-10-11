using System;
using System.Linq.Expressions;

namespace SECCS
{
    /// <summary>
    /// Represents the context of a type format.
    /// </summary>
    public class FormatContext
    {
        /// <summary>
        /// The type that is being (de)serialized.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The type of the buffer that is being used.
        /// </summary>
        public Type BufferType { get; }

        /// <summary>
        /// The expression for the buffer in use.
        /// </summary>
        public Expression Buffer { get; }

        /// <summary>
        /// The registered type formats.
        /// </summary>
        public IReadOnlyTypeFormatCollection Formats { get; }

        /// <summary>
        /// If <see cref="Type"/> is an interface type, <see cref="DeserializableType"/> will be set to the type that the user
        /// wants to instantiate when deserializing, if it was specified. Otherwise, null.
        /// </summary>
        public Type DeserializableType { get; }

        internal FormatContext(IReadOnlyTypeFormatCollection formats, Type type, Type bufferType, Expression buffer, Type concreteType = null)
        {
            this.Formats = formats;
            this.Type = type;
            this.BufferType = bufferType;
            this.Buffer = buffer;
            this.DeserializableType = concreteType;
        }

        /// <summary>
        /// Creates a copy of the current context and changes the type to <typeparamref name="T"/>.
        /// </summary>
        public FormatContext WithType<T>() => WithType(typeof(T));

        /// <summary>
        /// Creates a copy of the current context and changes the type to <paramref name="type"/>.
        /// </summary>
        public FormatContext WithType(Type type) => new FormatContext(Formats, type, BufferType, Buffer);
    }

    /// <summary>
    /// Represents the context of a type format that contains a value.
    /// </summary>
    public class FormatContextWithValue : FormatContext
    {
        /// <summary>
        /// The expression for the value being serialized.
        /// </summary>
        public Expression Value { get; }

        internal FormatContextWithValue(IReadOnlyTypeFormatCollection formats, Type type, Type bufferType, Expression buffer, Expression value, Type concreteType = null)
            : base(formats, type, bufferType, buffer, concreteType)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a copy of the current context and changes the type to <paramref name="type"/>.
        /// </summary>
        public new FormatContextWithValue WithType(Type type) => new FormatContextWithValue(Formats, type, BufferType, Buffer, Value);

        /// <summary>
        /// Creates a copy of the current context and changes the value to <paramref name="value"/>.
        /// </summary>
        public FormatContextWithValue WithValue(Expression value) => new FormatContextWithValue(Formats, Type, BufferType, Buffer, value);
    }
}
