namespace SECCS.Benchmarks
{
    public class DummyBuffer
    {
        public virtual void Write(int d) { }
        public virtual void Write(float d) { }
        public virtual void Write(double d) { }
        public virtual void Write(byte d) { }
        public virtual void Write(char d) { }
        public virtual void Write(string d) { }
        public virtual void Write(Primitive d) { }


        public virtual int ReadInt32() => 1;
        public virtual float ReadFloat() => 1.2f;
        public virtual double ReadDouble() => 2.1;
        public virtual byte ReadByte() => 255;
        public virtual char ReadChar() => 'a';
        public virtual string ReadString() => "a";
        public virtual Primitive ReadPrimitive() => null;
    }

    public class Primitive { }
}
