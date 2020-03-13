using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System.Collections.Generic;

namespace SECCS.Tests.Formats
{
    internal class ObjectFormatTest : BaseFormatTest<ObjectFormat<DummyBuffer>>
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
            bufferReaderMock.SetupPath<int>(nameof(TestClass1.Prop1));
            bufferReaderMock.SetupPath<string>(nameof(TestClass1.Prop2));
            bufferReaderMock.SetupPath<int>(nameof(TestClass1.Field1));
            bufferReaderMock.SetupPath<string>(nameof(TestClass1.Field2));

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), "");
            Format.Read(typeof(TestClass1), context);

            bufferReaderMock.Verify();
        }

        [Test]
        public void Read_ObjectWithConcreteType_CallsBufferReader()
        {
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(List<int>), It.IsAny<ReadFormatContext<DummyBuffer>>())).Returns(null).Verifiable();

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            Format.Read(typeof(TestClassConcrete), context);

            bufferReaderMock.Verify();
        }

        [Test]
        public void Write_Object_CallsBufferWriter()
        {
            var data = new TestClass1 { Prop1 = 123, Prop2 = "nice", Field1 = 42, Field2 = "adasd" };

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath(nameof(data.Prop1), data.Prop1);
            bufferWriterMock.SetupPath(nameof(data.Prop2), data.Prop2);
            bufferWriterMock.SetupPath(nameof(data.Field1), data.Field1);
            bufferWriterMock.SetupPath(nameof(data.Field2), data.Field2);

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "");
            Format.Write(data, context);

            bufferWriterMock.Verify();
        }
    }
}
