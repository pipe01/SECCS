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
            var contextMock = NewReadContextMock();
            contextMock.SetupPath<int>(nameof(TestClass1.Prop1));
            contextMock.SetupPath<string>(nameof(TestClass1.Prop2));
            contextMock.SetupPath<int>(nameof(TestClass1.Field1));
            contextMock.SetupPath<string>(nameof(TestClass1.Field2));

            Format.Read(typeof(TestClass1), contextMock.Object);

            contextMock.Verify();
        }

        [Test]
        public void Read_ObjectWithConcreteType_CallsBufferReader()
        {
            var contextMock = NewReadContextMock();
            contextMock.SetupPath<List<int>>(nameof(TestClassConcrete.List));

            Format.Read(typeof(TestClassConcrete), contextMock.Object);

            contextMock.Verify();
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
