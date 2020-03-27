using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SECCS.Tests.Utils
{
    public class MockReadContext : IReadFormatContext<DummyBuffer>, IDisposable
    {
        public DummyBuffer Reader { get; } = new DummyBuffer();
        public FormatOptions Options { get; } = new FormatOptions();

        private readonly IDictionary<(Type Type, string Path), object> ReadDictionary = new Dictionary<(Type Type, string Path), object>();

        public MockReadContext Setup(string path, object value)
        {
            ReadDictionary[(value.GetType(), path)] = value;

            return this;
        }

        public void Dispose()
        {
            Assert.AreEqual(0, ReadDictionary.Count, "Not all context setups have been called");
        }

        public object Read(Type type, PathGetter path, bool nullCheck = true)
        {
            if (ReadDictionary.TryGetValue((type, path.Path), out var val))
            {
                ReadDictionary.Remove((type, path.Path));
                return val;
            }

            throw new InvalidOperationException($"{type.Name} value not setup for {path.Path}");
        }
    }
}
