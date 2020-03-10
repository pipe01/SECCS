using System.IO;
using System.Text;

namespace Benchmarks
{
    public class MyBinaryWriter : BinaryWriter
    {
        private int position;

        public MyBinaryWriter(Stream output) : base(output)
        {
        }

        public MyBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public MyBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        protected MyBinaryWriter()
        {
        }

        public long Position
        {
            get => base.BaseStream.Position;
        }
    }
}
