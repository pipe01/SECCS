namespace SECCS
{
    public static class FormatCollectionExtensions
    {
        public static void Add<T, TReader>(this FormatCollection<IReadFormat<TReader>> formats, ReadDelegate<T, TReader> reader)
        {
            formats.Add(new DelegateReadFormat<T, TReader>(reader));
        }

        public static void Add<T, TWriter>(this FormatCollection<IWriteFormat<TWriter>> formats, WriteDelegate<T, TWriter> writer)
        {
            formats.Add(new DelegateWriteFormat<T, TWriter>(writer));
        }
    }
}
