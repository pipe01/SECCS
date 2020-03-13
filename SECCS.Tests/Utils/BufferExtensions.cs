using Moq;
using Moq.Language.Flow;
using SECCS.Formats;

namespace SECCS.Tests.Utils
{
    public static class BufferExtensions
    {
        public static IReturnsResult<IBufferReader<DummyBuffer>> SetupNullMarker(this Mock<IBufferReader<DummyBuffer>> buffer, bool isNull = false, bool invalid = false)
        {
            return buffer.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(byte), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path.EndsWith(ObjectFormat<DummyBuffer>.NullPath))))
                         .Returns((byte)(isNull ? 0 : invalid ? 2 : 1));
        }

        public static void SetupPath<T>(this Mock<IBufferReader<DummyBuffer>> buffer, string path)
        {
            buffer.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(T), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == "." + path))).Verifiable();
        }

        public static void SetupPath<T>(this Mock<IBufferWriter<DummyBuffer>> buffer, string path, T value, string message = null)
        {
            buffer.Setup(o => o.Serialize(It.IsAny<DummyBuffer>(), value, It.Is<WriteFormatContext<DummyBuffer>>(o => o.Path == "." + path))).Verifiable(message);
        }
    }
}
