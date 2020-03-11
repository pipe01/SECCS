﻿using Moq;
using NUnit.Framework;
using SECCS.Formats.Write;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Formats
{
    public class ArrayFormatWriterTest
    {
        private ArrayFormatWriter<DummyBuffer> Format => new ArrayFormatWriter<DummyBuffer>();

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
                new TestClass1 { Field1 = 123, Field2 = "nice" },
                new TestClass1 { Field1 = 42, Field2 = "foo" },
            }
        };

        [TestCaseSource(nameof(TestData))]
        public void Write_Array_CallsBufferWriter(TestClass1[] data)
        {
            var writer = new ArrayFormatWriter<DummyBuffer>();

            var bufferWriterMock = new Mock<IBufferWriter<DummyBuffer>>();
            bufferWriterMock.SetupPath("Length", data.Length, "Length not written");
            for (int i = 0; i < data.Length; i++)
            {
                bufferWriterMock.SetupPath($"[{i}]", data[i], $"Item {i} not written");
            }

            var context = new WriteFormatContext<DummyBuffer>(bufferWriterMock.Object, new DummyBuffer(), "");
            writer.Write(new DummyBuffer(), data, context);

            bufferWriterMock.Verify();
        }
    }
}