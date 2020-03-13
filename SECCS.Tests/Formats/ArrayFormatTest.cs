using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;

namespace SECCS.Tests.Formats
{
    public class ArrayFormatTest : BaseFormatTest<ArrayFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonArrayType_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1)));
        }

        [Test]
        public void CanFormat_ArrayType_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(TestClass1[])));
        }

        public static readonly TestClass1[][] TestData = new[]
        {
            new TestClass1[0],
            new[]
            {
                new TestClass1 { Prop1 = 123, Prop2 = "nice", Field1 = 56, Field2 = "asdoisajd" },
                new TestClass1 { Prop1 = 42, Prop2 = "foo", Field1 = 3948, Field2 = "soigjofdgij" },
            }
        };

        [TestCaseSource(nameof(TestData))]
        public void Write_Array_CallsBufferWriter(TestClass1[] data)
        {
            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath("Length", data.Length, "Length not written");
            for (int i = 0; i < data.Length; i++)
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
        public void Read_TestClassArray_CallsBufferReader(int arrayLength)
        {
            int timesCalled = 0;
            var buffer = new DummyBuffer();

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(int), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == ".Length")))
                            .Returns(arrayLength).Verifiable();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(TestClass1), It.IsAny<ReadFormatContext<DummyBuffer>>()))
                            .Callback(() => timesCalled++);

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            Format.Read(typeof(TestClass1[]), context);

            bufferReaderMock.Verify();
            Assert.AreEqual(arrayLength, timesCalled);
        }
    }
}
