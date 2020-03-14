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
        public void Write_Dictionary_CallsBufferWriter(IDictionary data)
        {
            var count = data.Cast<object>().Count();

            var buffer = new DummyBuffer();

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>(MockBehavior.Strict);
            bufferWriterMock.Setup(o => o.Serialize(buffer, It.IsAny<byte>(), It.Is<WriteFormatContext<DummyBuffer>>(o => o.Path.EndsWith("@Null"))));
            bufferWriterMock.Setup(o => o.Serialize(buffer, count, It.IsAny<WriteFormatContext<DummyBuffer>>())).Verifiable();

            int i = 0;
            foreach (var key in data.Keys)
            {
                int ii = i;

                bufferWriterMock.Setup(o => o.Serialize(buffer, key, It.Is<WriteFormatContext<DummyBuffer>>(o => o.Path == $".[{ii}].Key"))).Verifiable();
                bufferWriterMock.Setup(o => o.Serialize(buffer, data[key], It.Is<WriteFormatContext<DummyBuffer>>(o => o.Path == $".[{ii}].Value"))).Verifiable();

                i++;
            }

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, buffer, "");
            Format.Write(data, context);

            bufferWriterMock.Verify();
        }
    }
}
