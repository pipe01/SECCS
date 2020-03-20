using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Interfaces;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    internal class SeccsWriteableFormatTest : BaseFormatTest<SeccsWriteableFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonSeccsWriteable_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1), new FormatOptions()));
        }

        [Test]
        public void CanFormat_SeccsWriteable_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(Writeable1), new FormatOptions()));
        }

        [Test]
        public void Write_Writeable_CallsWritableWrite()
        {
            var buffer = new DummyBuffer();

            var writeableMock = new Mock<ISeccsWriteable<DummyBuffer>>();
            writeableMock.Setup(o => o.Write(buffer)).Verifiable();

            Format.Write(writeableMock.Object, new WriteFormatContext<DummyBuffer>(Mock.Of<IBufferWriter<DummyBuffer>>(), buffer, ""));

            writeableMock.Verify();
        }

        private class Writeable1 : ISeccsWriteable<DummyBuffer>
        {
            public void Write(DummyBuffer writer) { }
        }
    }
}
