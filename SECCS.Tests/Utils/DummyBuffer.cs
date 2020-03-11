using NUnit.Framework;

namespace SECCS.Tests.Utils
{
    public class DummyBuffer
    {
        public virtual void Write(int d) => TestContext.WriteLine(d);
        public virtual void Write(float d) => TestContext.WriteLine(d);
        public virtual void Write(double d) => TestContext.WriteLine(d);
        public virtual void Write(byte d) => TestContext.WriteLine(d);


        public virtual int ReadInt32() => 1;
        public virtual float ReadFloat() => 1.2f;
        public virtual double ReadDouble() => 2.1;
        public virtual byte ReadByte() => 255;
    }
}
