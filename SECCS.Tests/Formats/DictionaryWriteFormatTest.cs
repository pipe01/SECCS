using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SECCS.Tests.Formats
{
    internal class DictionaryWriteFormatTest : BaseFormatTest<DictionaryWriteFormat<DummyBuffer>>
    {
        [TestCase(typeof(Dictionary<int, int>))]
        [TestCase(typeof(IReadOnlyDictionary<int, int>))]
        public void CanFormat_Dictionary_True(Type type)
        {
            Assert.True(Format.CanFormat(type));
        }

        public static readonly object[] WriteData = new object[]
        {
            new Dictionary<int, int> { { 1, 2 }, { 2, 3 }, { 3, 4 } },
            new Dictionary<string, int>{ { "one", 1 }, { "two", 2 }, { "three", 3 } },
            new Dictionary<string, TestClass1>{ { "one", new TestClass1() }, { "two", new TestClass1() }, { "three", new TestClass1() } },
        };

        [TestCaseSource(nameof(WriteData))]
        public void Write_Dictionary_CallsBufferWriter(IEnumerable data)
        {
            var count = data.Cast<object>().Count();

            var buffer = new DummyBuffer();

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>(MockBehavior.Strict);
            bufferWriterMock.Setup(o => o.Serialize(buffer, count, It.IsAny<WriteFormatContext<DummyBuffer>>())).Verifiable();
            foreach (var item in data)
            {
                bufferWriterMock.Setup(o => o.Serialize(buffer, item, It.IsAny<WriteFormatContext<DummyBuffer>>())).Verifiable();
            }

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, buffer, "");
            Format.Write(data, context);

            bufferWriterMock.Verify();
        }
    }
}
