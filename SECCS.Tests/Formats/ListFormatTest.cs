using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SECCS.Tests.Formats
{
    internal class ListFormatTest : BaseFormatTest<ListFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonListType_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(object), new FormatOptions()));
        }

        [Test]
        public void CanFormat_ListTypeNonGeneric_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(ArrayList), new FormatOptions()));
        }

        [Test]
        public void CanFormat_ListType_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(List<TestClass1>), new FormatOptions()));
        }

        public static readonly object[] WriteListData = new object[]
        {
            new List<int>(),
            new List<int> { 1, 2, 3, 4 },
            new List<string> { "asd", "heasdasd", "foo" },
            new int[0],
            new[] { 1, 2, 3 },
            new[] { "one", "two", "three" }
        };

        [TestCaseSource(nameof(WriteListData))]
        public void Write_ListAndArray_CallsBufferWriter(IList data)
        {
            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Count", data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                contextMock.SetupPath($"[{i}]", data[i]);
            }

            Format.Write(data, contextMock.Object);

            contextMock.Verify();
        }

        public static object[] ReadListSource = new object[]
        {
            new List<int> { 1, 2, 3 },
            new List<string> { "asd", "sadsd", "lol" },
            new int[0],
            new[] { 1, 2, 3 },
            new[] { "one", "two", "three" }
        };

        [TestCaseSource(nameof(ReadListSource))]
        public void Read_ListAndArray_CallsBufferWriter(IList list)
        {
            using var contextMock = new MockReadContext();
            contextMock.Setup("Count", list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                contextMock.Setup($"[{i}]", list[i]);
            }

            var read = Format.Read(list.GetType(), contextMock);

            CollectionAssert.AreEqual(list, (IEnumerable)read);
        }
    }
}
