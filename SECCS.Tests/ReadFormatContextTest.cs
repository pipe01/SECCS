using Moq;
using NUnit.Framework;
using SECCS.Exceptions;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;

namespace SECCS.Tests
{
    public class ReadFormatContextTest
    {
        [Test]
        public void Read_NullDataWithReferenceType_ReturnsNull()
        {
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>(MockBehavior.Strict);
            bufferReaderMock.SetupNullMarker(true).Verifiable();

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            var data = context.Read<TestClass1>();

            Assert.AreEqual(null, data);
        }

        [Test]
        public void Read_InvalidMarkerWithReferenceType_Throws()
        {
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>(MockBehavior.Strict);
            bufferReaderMock.SetupNullMarker(invalid: true).Verifiable();

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");

            Assert.Throws(Is.TypeOf<FormattingException>().And.Message.EqualTo("Invalid null marker found: 2"),
                () => context.Read<TestClass1>());
        }


        private struct TestStruct
        {
#pragma warning disable 649
            public int ID;
#pragma warning restore 649

            public override bool Equals(object obj)
            {
                return obj is TestStruct t && t.ID == this.ID;
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }
    }
}
