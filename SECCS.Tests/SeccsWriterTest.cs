using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Utils;

namespace SECCS.Tests
{
    public class SeccsWriterTest
    {
        [Test]
        public void Serialize_TypeWithNoFormat_Throws()
        {
            var writer = new SeccsWriter<DummyBuffer>();
            writer.Formats.Clear();

            Assert.Throws<FormatNotFoundException>(() => writer.Serialize(new DummyBuffer(), new object()));
        }

        [Test]
        public void Serialize_TypeWithFormat_CallsWrite()
        {
            var writer = new SeccsWriter<DummyBuffer>();

            var formatMock = new Mock<IWriteFormat<DummyBuffer>>();
            formatMock.Setup(o => o.CanFormat(typeof(object), It.IsAny<FormatOptions>())).Returns(true).Verifiable();
            formatMock.Setup(o => o.Write(It.IsAny<object>(), It.IsAny<WriteFormatContext<DummyBuffer>>())).Callback(Assert.Pass).Verifiable();

            writer.Formats.Clear();
            writer.Formats.Add(formatMock.Object);

            writer.Serialize(new DummyBuffer(), new object());

            formatMock.Verify();
        }
    }
}
