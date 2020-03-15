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

            var bufferReaderMock = new Mock<IBufferReader<DummyBuffer>>();
            bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(int), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == ".Count"))).Returns(dicSize).Verifiable();
            for (int i = 0; i < dicSize; i++)
            {
                var ii = i;

                bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(float), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == $".[{ii}].Key"))).Returns(3.3f).Verifiable();
                bufferReaderMock.Setup(o => o.Deserialize(buffer, typeof(double), It.Is<ReadFormatContext<DummyBuffer>>(o => o.Path == $".[{ii}].Value"))).Returns(3.3).Verifiable();
            }

            var context = new ReadFormatContext<DummyBuffer>(bufferReaderMock.Object, buffer, "");
            Format.Read(typeof(Dictionary<float, double>), context);

            bufferReaderMock.Verify();
        }
    }
}
