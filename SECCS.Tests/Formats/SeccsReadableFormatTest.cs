using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Interfaces;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    public class SeccsReadableFormatTest : BaseFormatTest<SeccsReadableFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonSeccsReadable_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1)));
        }

        [Test]
        public void CanFormat_SeccsReadable_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(Readable1)));
        }

        [Test]
        public void Read_Readable_CallsReadableRead()
        {
            var buffer = new DummyBuffer();

            Format.Read(typeof(Readable1), new ReadFormatContext<DummyBuffer>(Mock.Of<IBufferReader<DummyBuffer>>(), buffer, ""));

            Assert.Fail("Read not called");
        }

        private class Readable1 : ISeccsReadable<DummyBuffer>
        {
            public void Read(DummyBuffer reader) => Assert.Pass();
        }
    }
}
