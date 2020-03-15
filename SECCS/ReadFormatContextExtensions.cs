namespace SECCS
{
    public static class ReadFormatContextExtensions
    {
        public static T Read<T>(this IReadFormatContext context, string path = "<>", bool nullCheck = true)
        {
            return (T)(context.Read(typeof(T), path, nullCheck) ?? default(T));
        }

    }
}
