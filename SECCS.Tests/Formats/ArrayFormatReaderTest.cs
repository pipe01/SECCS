using Moq;
using NUnit.Framework;
using SECCS.Formats.Read;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    public class ArrayFormatReaderTest : BaseFormatTest<ArrayFormatReader<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonArrayType_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1)));
        }

        [Test]
        public void Read_TestClassArray_CallsBufferWriter()
        {
            const int arrayLength = 2;

            var reader = new ObjectFormatReader<DummyBuffer>();
            int timesCalled = 0;

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(int), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == ".Length"))).Returns(arrayLength);
            bufferReaderMock.Setup(o => o.Deserialize(It.IsAny<DummyBuffer>(), typeof(TestClass1), It.IsAny<ReadFormatContext<DummyBuffer>>())).Callback(() => timesCalled++);

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), "");
            Format.Read(new DummyBuffer(), typeof(TestClass1[]), context);

            bufferReaderMock.Verify();
            Assert.AreEqual(arrayLength, timesCalled);
        }
    }
}
