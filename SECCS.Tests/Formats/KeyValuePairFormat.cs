using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Utils;
using System.Collections.Generic;

namespace SECCS.Tests.Formats
{
    internal class KeyValuePairFormat : BaseFormatTest<KeyValuePairFormat<DummyBuffer>>
    {
        [Test]
        public void CanFormat_KeyValuePair_True()
        {
            Assert.IsTrue(Format.CanFormat(typeof(KeyValuePair<int, int>), new FormatOptions()));
        }

        [Test]
        public void Write_KeyValuePair_CallsContext()
        {
            var kvp = new KeyValuePair<int, string>(123, "nice");

            var contextMock = NewWriteContextMock();
            contextMock.SetupPath("Key", kvp.Key);
            contextMock.SetupPath("Value", kvp.Value);

            Format.Write(kvp, contextMock.Object);

            contextMock.Verify();
        }

        [Test]
        public void Read_KeyValuePair_CallsContext()
        {
            var kvp = new KeyValuePair<int, string>(123, "nice");

            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Key", kvp.Key);
            contextMock.SetupPath("Value", kvp.Value);

            var read = Format.Read(typeof(KeyValuePair<int, string>), contextMock.Object);

            contextMock.Verify();
            Assert.AreEqual(kvp, read);
        }
    }
}
