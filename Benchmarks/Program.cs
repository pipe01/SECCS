using SECCS;
using System.IO;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var mem = new MemoryStream();
            var binWriter = new BinaryWriter(mem);
            var writer = new SeccsWriter<BinaryWriter>();

            writer.Serialize(binWriter, new Class
            {
                Number = 42,
                Byte = 255,
                Inner = new ClassInner
                {
                    InnerNumber = 69,
                    String = "asdasd"
                }
            });

            binWriter.Flush();
            mem.Position = 0;

            var binReader = new BinaryReader(mem);
            var reader = new SeccsReader<BinaryReader>();

            var d = reader.Deserialize<Class>(binReader);
        }
    }

    public class Class
    {
        public int Number { get; set; }
        public byte Byte { get; set; }
        public ClassInner Inner { get; set; }
    }

    public class ClassInner
    {
        public int InnerNumber { get; set; }
        public string String { get; set; }
    }
}