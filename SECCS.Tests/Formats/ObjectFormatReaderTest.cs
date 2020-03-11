using Moq;
using NUnit.Framework;
using SECCS.Formats.Read;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    public class ObjectFormatReaderTest : BaseFormatTest<ObjectFormatReader<DummyBuffer>>
    {
        [Test]
        public void CanFormat_AnyType_ReturnsTrue()
        {
            Assert.IsTrue(Format.CanFormat(typeof(object)));
        }

        [Test]
        public void Read_TestClass_CallsBufferWriter()
        {
            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.SetupPath<int>(nameof(TestClass1.Field1));
            bufferReaderMock.SetupPath<string>(nameof(TestClass1.Field2));

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, new DummyBuffer(), "");
            Format.Read(new DummyBuffer(), typeof(TestClass1), context);

            bufferReaderMock.Verify();
        }
    }
}
