using Moq;
using Moq.Language.Flow;
using SECCS.Formats;
using System;

namespace SECCS.Tests.Utils
{
    public static class MockExtensions
    {
        public static IReturnsResult<IBufferReader<DummyBuffer>> SetupNullMarker(this Mock<IBufferReader<DummyBuffer>> buffer, bool isNull = false, bool invalid = false)
        {
            return buffer.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(byte), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path.EndsWith(ObjectFormat<DummyBuffer>.NullPath))))
                         .Returns((byte)(isNull ? 0 : invalid ? 2 : 1));
        }

        public static void SetupPath<T>(this Mock<IReadFormatContext<DummyBuffer>> contextMock, string path, T value = default) 
            => contextMock.SetupPath(typeof(T), path, value);
        
        public static void SetupPath(this Mock<IReadFormatContext<DummyBuffer>> contextMock, Type type, string path, object value = null)
        {
            contextMock.Setup(o => o.Read(type, path, It.IsAny<bool>())).Returns(value).Verifiable();
        }

        public static void SetupPath<T>(this Mock<IWriteFormatContext<DummyBuffer>> contextMock, string path, T value, string message = null)
        {
            contextMock.Setup(o => o.Write(value, path, It.IsAny<bool>())).ReturnsNull().Verifiable(message);
        }

        public static IReturnsResult<T> ReturnsNull<T, TResult>(this ISetup<T, TResult> setup) where T : class
            => setup.Returns(() => default);
    }
}
