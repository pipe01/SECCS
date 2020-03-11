using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests
{
    public class SeccsWriterTest
    {
        [Test]
        public void Serialize_TypeWithNoFormat_Throws()
        {
            var writer = new SeccsWriter<DummyBuffer>();

            Assert.Throws<FormatNotFoundException>(() => writer.Serialize(new DummyBuffer(), new object()));
        }

        [Test]
        public void Serialize_TypeWithFormat_CallsWrite()
        {
            var writer = new SeccsWriter<DummyBuffer>();

            var formatMock = new Mock<IWriteFormat<DummyBuffer>>();
            formatMock.Setup(o => o.CanFormat(typeof(object))).Returns(true).Verifiable();
            formatMock.Setup(o => o.Write(It.IsAny<DummyBuffer>(), It.IsAny<object>(), It.IsAny<WriteFormatContext<DummyBuffer>>())).Callback(Assert.Pass).Verifiable();

            writer.WriteFormats.Add(formatMock.Object);

            writer.Serialize(new DummyBuffer(), new object());

            formatMock.Verify();
        }
    }
}
