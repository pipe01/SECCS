using Moq;
using Moq.Language.Flow;

namespace SECCS.Tests.Utils
{
    public static class BufferExtensions
    {
        public static void SetupPath<T>(this Mock<IBufferReader<DummyBuffer>> buffer, string path)
        {
            buffer.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(T), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == "." + path))).Verifiable();
        }

        public static void SetupPath<T>(this Mock<IBufferWriter<DummyBuffer>> buffer, string path, T value)
        {
            buffer.Setup(o => o.Serialize(It.IsAny<DummyBuffer>(), value, It.Is<WriteFormatContext<DummyBuffer>>(o => o.Path == "." + path))).Verifiable();
        }
    }
}
