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
            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Count", data.Count);
            for (int i = 0; i < data.Count; i++)
            {
                contextMock.SetupPath($"[{i}]", data[i]);
            }

            Format.Write(data, contextMock.Object);

            contextMock.Verify();
        }

        public static object[] ReadSource = new object[]
        {
            new List<int> { 1, 2, 3 },
            new List<string> { "asd", "sadsd", "lol" }
        };

        [TestCaseSource(nameof(ReadSource))]
        public void Read_List_CallsBufferWriter(IList list)
        {
            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Count", list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                contextMock.SetupPath($"[{i}]", list[i]);
            }

            var read = Format.Read(list.GetType(), contextMock.Object);

            contextMock.Verify();
            CollectionAssert.AreEqual(list, (IEnumerable)read);
        }
    }
}
