using Moq;
using NUnit.Framework;
using SECCS.Formats.Read;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Formats
{
    public class ObjectFormatReaderTest
    {
        [Test]
        public void CanFormat_AnyType_ReturnsTrue()
        {
            Assert.IsTrue(new ObjectFormatReader<DummyBuffer>().CanFormat(typeof(object)));
        }

        [Test]
        public void Write_Object_CallsBufferWriter()
        {
            var reader = new ObjectFormatReader<DummyBuffer>();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(int), It.IsAny<ReadFormatContext<DummyBuffer>>())).Verifiable();
            bufferReaderMock.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(string), It.IsAny<ReadFormatContext<DummyBuffer>>())).Verifiable();

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), ".");
            reader.Read(new DummyBuffer(), typeof(TestClass1), context);

            bufferReaderMock.Verify();
        }
    }
}
