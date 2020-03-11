using Moq;
using NUnit.Framework;
using SECCS.Formats.Write;
using SECCS.Tests.Classes;
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

            var data = new TestClass1 { Field1 = 123, Field2 = "nice" };

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath(nameof(data.Field1), data.Field1);
            bufferWriterMock.SetupPath(nameof(data.Field2), data.Field2);

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "");
            writer.Write(new DummyBuffer(), data, context);

            bufferWriterMock.Verify();
        }
    }
}
