using NUnit.Framework;
using SECCS.Formats;
using SECCS.Tests.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

#if NETCOREAPP
using System.Collections.Immutable;
#endif

namespace SECCS.Tests.Formats
{
    internal class DictionaryReadFormatTest : BaseFormatTest<DictionaryReadFormat<DummyBuffer>>
    {
#if NETCOREAPP
        [TestCase(typeof(ImmutableDictionary<int, int>))]
#endif
        [TestCase(typeof(Dictionary<int, int>))]
        public void CanFormat_TypeIsDictionary_True(Type type)
        {
            Assert.IsTrue(Format.CanFormat(type, new FormatOptions()));
        }

        [TestCase(typeof(IDictionary))]
        [TestCase(typeof(IDictionary<int, int>))]
        [TestCase(typeof(IReadOnlyDictionary<int, int>))]
        [TestCase(typeof(OrderedDictionary))]
        public void CanFormat_TypeIsNotReadable_False(Type type)
        {
            Assert.IsFalse(Format.CanFormat(type, new FormatOptions()));
        }

        [Test]
        public void Read_Dictionary_CallsBufferReader()
        {
            var dic = new Dictionary<int, string>
            {
                [1] = "one",
                [2] = "two",
                [3] = "three"
            };

            var contextMock = NewReadContextMock();
            contextMock.SetupPath("Count", dic.Count);

            int i = 0;
            foreach (var item in dic)
            {
                contextMock.SetupPath($"[{i}].Key", item.Key);
                contextMock.SetupPath($"[{i}].Value", item.Value);

                i++;
            }

            var read = Format.Read(dic.GetType(), contextMock.Object);

            contextMock.Verify();
            CollectionAssert.AreEqual(dic, (IEnumerable)read);
        }
    }
}
