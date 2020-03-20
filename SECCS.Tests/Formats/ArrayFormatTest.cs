using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;
using System.Collections;

namespace SECCS.Tests.Formats
{
    internal class ArrayFormatTest : BaseFormatTest<ArrayFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_NonArrayType_False()
        {
            Assert.IsFalse(Format.CanFormat(typeof(TestClass1), new FormatOptions()));
        }

        [Test]
        public void CanFormat_ArrayType_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(TestClass1[]), new FormatOptions()));
        }

        public static readonly object[] TestData = new object[]
        {
            new TestClass1[0],
            new[]
            {
                new TestClass1 { Prop1 = 123, Prop2 = "nice", Field1 = 56, Field2 = "asdoisajd" },
                new TestClass1 { Prop1 = 42, Prop2 = "foo", Field1 = 3948, Field2 = "soigjofdgij" },
            },
            new[] { 1, 2, 3, 4 },
            new[] { "asd", "nice", "123" }
        };

        [TestCaseSource(nameof(TestData))]
        public void Write_Array_CallsBufferWriter(Array data)
        {
            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Length", data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                contextMock.SetupPath($"[{i}]", data.GetValue(i));
            }

            Format.Write(data, contextMock.Object);

            contextMock.Verify();
        }

        [Test]
        public void Read_TestClassArray_CallsBufferReader()
        {
            var array = new[] { 1, 2, 3, 4, 5 };

            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Length", array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                contextMock.SetupPath($"[{i}]", array[i]);
            }

            var read = Format.Read(array.GetType(), contextMock.Object);

            contextMock.Verify();
            CollectionAssert.AreEqual(array, (IList)read);
        }
    }
}
