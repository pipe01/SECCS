using Moq;
using SECCS.Tests.Utils;

namespace SECCS.Tests
{
    public class BaseFormatTest<T> where T : new()
    {
        protected T Format => new T();

        protected Mock<IReadFormatContext<DummyBuffer>> NewReadContextMock() => new Mock<IReadFormatContext<DummyBuffer>>(MockBehavior.Strict);
        protected Mock<IWriteFormatContext<DummyBuffer>> NewWriteContextMock() => new Mock<IWriteFormatContext<DummyBuffer>>(MockBehavior.Strict);
    }
}
