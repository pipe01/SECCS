using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Classes;
using SECCS.Tests.Utils;
using System.Collections.Generic;

namespace SECCS.Tests.Formats
{
    internal class ObjectFormatTest : BaseFormatTest<ObjectFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_AnyType_ReturnsTrue()
        {
            Assert.IsTrue(Format.CanFormat(typeof(object), new FormatOptions()));
        }

        [Test]
        public void Read_TestClass_CallsBufferReader()
        {
            var data = new TestClass1()
            {
                Prop1 = 123,
                Prop2 = "asd",
                Field1 = 321,
                Field2 = "foo"
            };

            using var contextMock = new MockReadContext();
            contextMock.Setup(nameof(TestClass1.Prop1), data.Prop1);
            contextMock.Setup(nameof(TestClass1.Prop2), data.Prop2);
            contextMock.Setup(nameof(TestClass1.Field1), data.Field1);
            contextMock.Setup(nameof(TestClass1.Field2), data.Field2);

            var read = (TestClass1)Format.Read(typeof(TestClass1), contextMock);

            Assert.AreEqual(data.Prop1, read.Prop1);
            Assert.AreEqual(data.Prop2, read.Prop2);
            Assert.AreEqual(data.Field1, read.Field1);
            Assert.AreEqual(data.Field2, read.Field2);
        }

        [Test]
        public void Read_ObjectWithConcreteType_CallsBufferReader()
        {
            using var contextMock = new MockReadContext();
            contextMock.Setup(nameof(TestClassConcrete.List), new List<int>());

            Format.Read(typeof(TestClassConcrete), contextMock);
        }

        [Test]
        public void Write_Object_CallsBufferWriter()
        {
            var data = new TestClass1 { Prop1 = 123, Prop2 = "nice", Field1 = 42, Field2 = "adasd" };

            var contextMock = NewWriteContextMock();
            contextMock.SetupPath(nameof(data.Prop1), data.Prop1);
            contextMock.SetupPath(nameof(data.Prop2), data.Prop2);
            contextMock.SetupPath(nameof(data.Field1), data.Field1);
            contextMock.SetupPath(nameof(data.Field2), data.Field2);

            Format.Write(data, contextMock.Object);

            contextMock.Verify();
        }

        private class NullClass
        {
#pragma warning disable 649
            public string IsNull;
#pragma warning restore 649
        }
    }
}
