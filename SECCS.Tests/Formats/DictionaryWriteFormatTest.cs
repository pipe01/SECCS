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
            Assert.True(Format.CanFormat(type, new FormatOptions()));
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

            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Count", count);

            int i = 0;
            foreach (var key in data.Keys)
            {
                contextMock.SetupPath($"[{i}].Key", key);
                contextMock.SetupPath($"[{i}].Value", data[key]);

                i++;
            }

            Format.Write(data, contextMock.Object);

            contextMock.Verify();
        }
    }
}
