using NUnit.Framework;

namespace SECCS.Tests.Utils
{
    public class DummyBuffer
    {
        public virtual void Write(int d) => TestContext.WriteLine(d);
        public virtual void Write(float d) => TestContext.WriteLine(d);
        public virtual void Write(double d) => TestContext.WriteLine(d);
        public virtual void Write(byte d) => TestContext.WriteLine(d);
    }
}
