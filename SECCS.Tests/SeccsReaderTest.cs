using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Utils;
using System;

namespace SECCS.Tests
{
    public class SeccsReaderTest
    {
        [Test]
        public void Deserialize_TypeWithNoFormat_Throws()
        {
            var reader = new SeccsReader<DummyBuffer>();
            reader.ReadFormats.Clear();

            Assert.Throws<FormatNotFoundException>(() => reader.Deserialize<object>(new DummyBuffer()));
        }

        [Test]
        public void Deserialize_TypeWithFormat_CallsRead()
        {
            var formatMock = new Mock<IReadFormat<DummyBuffer>>();
            formatMock.Setup(o => o.CanFormat(typeof(object))).Returns(true).Verifiable();
            formatMock.Setup(o => o.Read(It.IsAny<Type>(), It.IsAny<ReadFormatContext<DummyBuffer>>())).Verifiable();

            var reader = new SeccsReader<DummyBuffer>();
            reader.ReadFormats.Clear();
            reader.ReadFormats.Add(formatMock.Object);

            reader.Deserialize(new DummyBuffer(), typeof(object));

            formatMock.Verify();
        }
    }
}
