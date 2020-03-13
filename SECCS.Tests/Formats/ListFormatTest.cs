using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System.Collections;
using System.Collections.Generic;

namespace SECCS.Tests.Formats
{
    internal class ListFormatTest : BaseFormatTest<ListFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonListType_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(object)));
        }

        [Test]
        public void CanFormat_ListTypeNonGeneric_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(ArrayList)));
        }

        [Test]
        public void CanFormat_ListType_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(List<TestClass1>)));
        }

        public static readonly object[] TestData = new object[]
        {
            new List<int>(),
            new List<int> { 1, 2, 3, 4 },
            new List<string> { "asd", "heasdasd", "foo" }
        };

        [TestCaseSource(nameof(TestData))]
        public void Write_List_CallsBufferWriter(IList data)
        {
            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath("Count", data.Count, "Count not written");
            for (int i = 0; i < data.Count; i++)
            {
                bufferWriterMock.SetupPath($"[{i}]", data[i], $"Item {i} not written");
            }

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "");
            Format.Write(data, context);

            bufferWriterMock.Verify();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(200)]
        public void Read_List_CallsBufferWriter(int listSize)
        {
            int timesCalled = 0;
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(int), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == ".Count")))
                            .Returns(listSize).Verifiable();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(TestClass1), It.IsAny<ReadFormatContext<DummyBuffer>>()))
                            .Callback(() => timesCalled++);

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            Format.Read(typeof(List<TestClass1>), context);

            bufferReaderMock.Verify();
            Assert.AreEqual(listSize, timesCalled);
        }
    }
}
