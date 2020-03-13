using Moq;
using NUnit.Framework;
using SECCS.Formats.MiscTypes;
using SECCS.Tests.Utils;
using System;
using System.Linq;

namespace SECCS.Tests.Formats.MiscTypes
{
    internal class GuidFormat : BaseFormatTest<GuidFormat<DummyBuffer>>
    {
        [Test]
        public void Write_Guid_CallsBufferWriter()
        {
            var bytes = Enumerable.Repeat((byte)0, 16).ToArray();

            var buffer = new DummyBuffer();

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath(GuidFormat<DummyBuffer>.BytesPath, bytes);

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, buffer, "");
            Format.Write(new Guid(bytes), context);

            bufferWriterMock.Verify();
        }

        [Test]
        public void Read_Guid_CallsBufferReader()
        {
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.SetupNullMarker();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(byte[]), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == "." + GuidFormat<DummyBuffer>.BytesPath)))
                .Returns(Enumerable.Repeat((byte)0, 16).ToArray())
                .Verifiable();

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            Format.Read(typeof(Guid), context);

            bufferReaderMock.Verify();
        }
    }
}
