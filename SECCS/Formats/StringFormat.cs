using System;

namespace SECCS.Formats
{
    internal class StringFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => type == typeof(string);

        public object Read(Type type, IReadFormatContext<T> context)
        {
            return new string(context.Read<char[]>("CharArray"));
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            context.Write(((string)obj).ToCharArray(), "CharArray");
        }
    }
}
