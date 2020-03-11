using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests
{
    public class SeccsReaderTest
    {
        [Test]
        public void Deserialize_TypeWithNoFormat_Throws()
        {
            var writer = new SeccsReader<DummyBuffer>();

            Assert.Throws<FormatNotFoundException>(() => writer.Deserialize<object>(new DummyBuffer()));
        }

        [Test]
        public void Deserialize_TypeWithFormat_CallsRead()
        {
            var reader = new SeccsReader<DummyBuffer>();

            var formatMock = new Mock<IReadFormat<DummyBuffer>>();
            formatMock.Setup(o => o.CanFormat(typeof(object))).Returns(true).Verifiable();
            formatMock.Setup(o => o.Read(It.IsAny<DummyBuffer>(), It.IsAny<Type>(), It.IsAny<ReadFormatContext<DummyBuffer>>())).Verifiable();

            reader.ReadFormats.Add(formatMock.Object);

            reader.Deserialize(new DummyBuffer(), typeof(object));

            formatMock.Verify();
        }
    }
}
