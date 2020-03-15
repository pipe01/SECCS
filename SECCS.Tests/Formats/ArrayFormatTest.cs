using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;

namespace SECCS.Tests.Formats
{
    internal class ArrayFormatTest : BaseFormatTest<ArrayFormat<DummyBuffer>>
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

        [TestCase(0, typeof(int))]
        [TestCase(0, typeof(TestClass1))]
        [TestCase(1, typeof(int))]
        [TestCase(1, typeof(TestClass1))]
        [TestCase(2, typeof(int))]
        [TestCase(2, typeof(TestClass1))]
        [TestCase(10, typeof(int))]
        [TestCase(10, typeof(TestClass1))]
        public void Read_TestClassArray_CallsBufferReader(int arrayLength, Type elementType)
        {
            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Length", arrayLength);
            for (int i = 0; i < arrayLength; i++)
            {
                contextMock.SetupPath(elementType, $"[{i}]");
            }

            Format.Read(elementType.MakeArrayType(1), contextMock.Object);

            contextMock.Verify();
        }
    }
}
