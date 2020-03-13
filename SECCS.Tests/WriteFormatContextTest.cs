using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;

namespace SECCS.Tests
{
    public class WriteFormatContextTest
    {
        [Test]
        public void Write_BufferWriterThrows_ThrowsWrapped()
        {
            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.Setup(o => o.Serialize(It.IsAny<DummyBuffer>(), It.IsAny<object>(), It.IsAny<WriteFormatContext<DummyBuffer>>())).Throws(new Exception("Inner exception"));

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "Path");

            Assert.Throws(Is.TypeOf<FormattingException>()
                            .And.Message.EqualTo($"Failed to write object of type {typeof(TestClass1)} at path Path.InnerPath")
                            .And.InnerException.Message.EqualTo("Inner exception"),
                () => context.Write(new TestClass1(), "InnerPath"));
        }

        [Test]
        public void Read_BufferReaderThrows_ThrowsWrapped()
        {
            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), It.IsAny<Type>(), It.IsAny<ReadFormatContext<DummyBuffer>>())).Throws(new Exception("Inner exception"));

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), "Path");

            Assert.Throws(Is.TypeOf<FormattingException>()
                            .And.Message.EqualTo($"Failed to read type {typeof(TestClass1)} at path Path.InnerPath")
                            .And.InnerException.Message.EqualTo("Inner exception"),
                () => context.Read(typeof(TestClass1), "InnerPath"));
        }
    }
}
