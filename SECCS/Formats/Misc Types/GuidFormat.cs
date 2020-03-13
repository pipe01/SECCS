using System;

namespace SECCS.Formats.MiscTypes
{
    public class GuidFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        internal const string BytesPath = "GuidBytes";

        public bool CanFormat(Type type) => type == typeof(Guid);

        public object Read(Type type, ReadFormatContext<T> context)
        {
            var guidBytes = context.Read<byte[]>(BytesPath);

            return new Guid(guidBytes);
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            var guid = (Guid)obj;

            context.Write(guid.ToByteArray(), BytesPath);
        }
    }
}
