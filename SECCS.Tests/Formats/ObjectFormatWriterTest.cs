using Moq;
using NUnit.Framework;
using SECCS.Formats.Write;
using SECCS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Formats
{
    public class ObjectFormatWriterTest
    {
        [Test]
        public void CanFormat_AnyType_ReturnsTrue()
        {
            Assert.IsTrue(new ObjectFormatWriter<DummyBuffer>().CanFormat(typeof(object)));
        }

        [Test]
        public void Write_Object_CallsBufferWriter()
        {
            var writer = new ObjectFormatWriter<DummyBuffer>();

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.Setup(o => o.Serialize(It.IsAny<DummyBuffer>(), It.IsAny<object>(), It.IsAny<WriteFormatContext<DummyBuffer>>()))
                .Callback(Assert.Pass);

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), ".");
            writer.Write(new DummyBuffer(), new TestClass1(), context);

            Assert.Fail("Buffer not called");
        }

        private class TestClass1
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
        }
    }
}
