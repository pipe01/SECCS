namespace SECCS
{
    public static class BufferFormatterExtensions
    {
        /// <summary>
        /// Serializes <paramref name="obj"/> into <typeparamref name="TBuffer"/> using type formats.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="buffer">The buffer to serialize the object into</param>
        /// <param name="obj">The object to serialize</param>
        public static void Serialize<T, TBuffer>(this IBufferFormatter<TBuffer> formatter, TBuffer buffer, T obj)
            => formatter.Serialize(buffer, obj, typeof(T));

        /// <summary>
        /// Reads a <typeparamref name="T"/> from a <paramref name="buffer"/> of type <typeparamref name="TBuffer"/>.
        /// The type of <typeparamref name="T"/> can be inferred from the first parameter, which allows you to
        /// pass in an anonymous object with the fields that you want to deserialize. For example:
        /// <code>
        /// DeserializeAnonymousObject(new { Foo = "asd" }, buffer);
        /// </code>
        /// </summary>
        /// <typeparam name="T">The type of the object to read</typeparam>
        /// <param name="anonymousTypeObject">The anonymous object whose type will be inferred</param>
        /// <param name="buffer">The buffer to read from</param>
        public static T DeserializeAnonymousObject<T, TBuffer>(this IBufferFormatter<TBuffer> formatter, T anonymousTypeObject, TBuffer buffer)
            => formatter.Deserialize<T>(buffer);
    }
}
