using System;

namespace SECCS.Formats.MiscTypes
{
    internal class GuidFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        internal const string BytesPath = "GuidBytes";

        public bool CanFormat(Type type) => type == typeof(Guid);

        public object Read(Type type, IReadFormatContext<T> context)
        {
            var guidBytes = context.Read<byte[]>(BytesPath);

            return new Guid(guidBytes);
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            var guid = (Guid)obj;

            context.Write(guid.ToByteArray(), BytesPath);
        }
    }
}
