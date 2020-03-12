using SECCS;
using System.Collections.Generic;
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
                List = new List<int>
                {
                    1, 2, 3, 4
                },
                List2 = new List<ClassInner>
                {
                    new ClassInner { InnerNumber = 13, String = "one" },
                    new ClassInner { InnerNumber = 13, String = "two" },
                    new ClassInner { InnerNumber = 13, String = "three" },
                },
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
        public List<int> List { get; set; }
        public List<ClassInner> List2 { get; set; }

        public ClassInner Inner { get; set; }
    }

    public class ClassInner
    {
        public int InnerNumber { get; set; }
        public string String { get; set; }
    }
}