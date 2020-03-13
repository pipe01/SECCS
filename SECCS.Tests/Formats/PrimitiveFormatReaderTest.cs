using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    internal class PrimitiveFormatReaderTest : BaseFormatTest<PrimitiveFormatReader<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonPrimitive_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1)));
        }

        [Test]
        public void Read_Int32_ReadsFromDummy()
        {
            var dummyBufferMock = new Mock<DummyBuffer>();
            dummyBufferMock.Setup(o => o.ReadInt32()).Verifiable();

            Format.Read(typeof(int), new ReadFormatContext<DummyBuffer>(Mock.Of<IBufferReader<DummyBuffer>>(), dummyBufferMock.Object, ""));

            dummyBufferMock.Verify();
        }
    }
}
