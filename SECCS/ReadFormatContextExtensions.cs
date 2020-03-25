using System;

namespace SECCS
{
    public static class ReadFormatContextExtensions
    {
        public static T Read<T>(this IReadFormatContext context, string path = "<>", bool nullCheck = true)
        {
            return (T)(context.Read(typeof(T), path, nullCheck) ?? default(T));
        }

        public static object Read(this IReadFormatContext context, Type type, string path = "<>", bool nullCheck = true)
            => context.Read(type, new PathGetter(path, null), nullCheck);
    }
}
