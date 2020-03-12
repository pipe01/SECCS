using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    public class ObjectFormatReaderTest : BaseFormatTest<ObjectFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_AnyType_ReturnsTrue()
        {
            Assert.IsTrue(Format.CanFormat(typeof(object)));
        }

        [Test]
        public void Read_TestClass_CallsBufferReader()
        {
            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.SetupPath<int>(nameof(TestClass1.Field1));
            bufferReaderMock.SetupPath<string>(nameof(TestClass1.Field2));

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), "");
            Format.Read(typeof(TestClass1), context);

            bufferReaderMock.Verify();
        }


        [Test]
        public void Write_Object_CallsBufferWriter()
        {
            var data = new TestClass1 { Field1 = 123, Field2 = "nice" };

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath(nameof(data.Field1), data.Field1);
            bufferWriterMock.SetupPath(nameof(data.Field2), data.Field2);

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "");
            Format.Write(data, context);

            bufferWriterMock.Verify();
        }
    }
}
