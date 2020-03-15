using Moq;
using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Formats
{
    public class DictionaryReadFormatTest : BaseFormatTest<DictionaryReadFormat<DummyBuffer>>
    {
        [TestCase(typeof(Dictionary<int, int>))]
        [TestCase(typeof(ImmutableDictionary<int, int>))]
        public void CanFormat_TypeIsDictionary_True(Type type)
        {
            Assert.IsTrue(Format.CanFormat(type));
        }

        [TestCase(typeof(IDictionary))]
        [TestCase(typeof(IDictionary<int, int>))]
        [TestCase(typeof(IReadOnlyDictionary<int, int>))]
        [TestCase(typeof(OrderedDictionary))]
        public void CanFormat_TypeIsNotReadable_False(Type type)
        {
            Assert.IsFalse(Format.CanFormat(type));
        }

        [Test]
        public void Read_Dictionary_CallsBufferReader()
        {
            const int dicSize = 3;

            var buffer = new DummyBuffer();

            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Count", dicSize);
            for (int i = 0; i < dicSize; i++)
            {
                contextMock.SetupPath($"[{i}].Key", 3.3f);
                contextMock.SetupPath($"[{i}].Value", 3.3);
            }

            Format.Read(typeof(Dictionary<float, double>), contextMock.Object);

            contextMock.Verify();
        }
    }
}
