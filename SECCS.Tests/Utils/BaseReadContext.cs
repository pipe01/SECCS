using System;

namespace SECCS.Tests.Utils
{
    public abstract class BaseReadContext : IReadFormatContext<DummyBuffer>
    {
        public DummyBuffer Reader { get; set; }

        public FormatOptions Options { get; } = new FormatOptions();

        public object Read(Type type, string path = "<>", bool nullCheck = true)
        {
            throw new NotImplementedException();
        }

        public T Read<T>(string path = "<>", bool nullCheck = true)
        {
            throw new NotImplementedException();
        }
    }
}
