using Moq;
using Moq.Language.Flow;

namespace SECCS.Tests.Utils
{
    public static class BufferReaderExtensions
    {
        public static void SetupPath<T>(this Mock<IBufferReader<DummyBuffer>> buffer, string path)
        {
            buffer.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(T), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == path)))
                .Verifiable();
        }
    }
}
