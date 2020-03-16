using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Utils;
using System;

namespace SECCS.Tests.Formats
{
    [TestOf(typeof(TupleFormat<>))]
    internal class TupleFormatTest : BaseFormatTest<TupleFormat<DummyBuffer>>
    {
        public static readonly object[] CanFormatCases = new object[]
        {
            (1, 2, 3, 4),
            new ValueTuple(),
            ValueTuple.Create(1, "asd")
        };

        [TestCaseSource(nameof(CanFormatCases))]
        public void CanFormat_ValueTuple_True(object obj)
        {
            Assert.IsTrue(Format.CanFormat(obj.GetType()));
        }

        [Test]
        public void Read_Tuple_CallsContext()
        {
            var expected = (123, "nice", 3.1);

            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Length", 3);
            contextMock.SetupPath("Item1", expected.Item1);
            contextMock.SetupPath("Item2", expected.Item2);
            contextMock.SetupPath("Item3", expected.Item3);

            var result = Format.Read(expected.GetType(), contextMock.Object);
            Assert.AreEqual(expected, result);

            contextMock.Verify();
        }

        [Test]
        public void Write_Tuple_CallsContext()
        {
            var tuple = (123, "nice", 3.1);

            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Length", 3);
            contextMock.SetupPath("Item1", tuple.Item1);
            contextMock.SetupPath("Item2", tuple.Item2);
            contextMock.SetupPath("Item3", tuple.Item3);

            Format.Write(tuple, contextMock.Object);

            contextMock.Verify();
        }
    }
}
