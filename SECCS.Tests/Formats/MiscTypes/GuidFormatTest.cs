using Moq;
using NUnit.Framework;
using SECCS.Formats.MiscTypes;
using SECCS.Tests.Utils;
using System;
using System.Linq;

namespace SECCS.Tests.Formats.MiscTypes
{
    internal class GuidFormatTest : BaseFormatTest<GuidFormat<DummyBuffer>>
    {
        [Test]
        public void Write_Guid_CallsBufferWriter()
        {
            var contextMock = NewWriteContextMock();
            contextMock.Setup(o => o.Write(It.Is<byte[]>(i => i.All(j => j == 0)), GuidFormat<DummyBuffer>.BytesPath, It.IsAny<bool>())).ReturnsNull().Verifiable();

            Format.Write(new Guid(), contextMock.Object);

            contextMock.Verify();
        }

        [Test]
        public void Read_Guid_CallsBufferReader()
        {
            using var contextMock = new MockReadContext();
            contextMock.Setup(GuidFormat<DummyBuffer>.BytesPath, new byte[16]);

            Format.Read(typeof(Guid), contextMock);
        }
    }
}
