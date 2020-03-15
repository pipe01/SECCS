using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Tests.Utils
{
    public abstract class BaseReadContext : IReadFormatContext<DummyBuffer>
    {
        public DummyBuffer Reader { get; set; }

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
