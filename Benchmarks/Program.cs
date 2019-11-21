using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SECCS;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var t = new Test2
            {
                Dic = new Dictionary<int, int>
                {
                    [5] = 123,
                    [8] = 42
                }
            };

            using var mem = new MemoryStream();
            using var writer = new BinaryWriter(mem);
            var opt = new SeccsOptions { WriteHeader = false, WriteStructureSignature = false, CheckHeader = false, CheckStructureSignature = false };
            var formatter = new SeccsFormatter<BinaryWriter>(TypeFormat.GetFromReadAndWrite<BinaryWriter>(), opt);

            formatter.Serialize(writer, t);
            writer.Flush();
            mem.Flush();
            mem.Position = 0;

            var readFormatter = new SeccsFormatter<BinaryReader>(TypeFormat.GetFromReadAndWrite<BinaryReader>(), opt);
            using var reader = new BinaryReader(mem);

            var obj = readFormatter.Deserialize<Test2>(reader);
        }
    }

    public class Test2
    {
        [ConcreteType(typeof(Dictionary<int, int>))]
        public IReadOnlyDictionary<int, int> Dic;
    }

    public class Test
    {
        public Nested[] List;
    }

    public class Nested
    {
        public string Str;
        public int Hello;
    }

    [MessagePackObject]
    public class TestClass
    {
        [Key(1)]
        public List<int> List;

        [Key(2)]
        public (int, string) Tuple;

        [Key(3)]
        public KeyValuePair<int, string> KVP;

        [Key(4)]
        public int[][] Two;

        [Key(5)]
        public Dictionary<int, bool> Dic;
    }

    public class SerializeBenchmark
    {
        private readonly TestClass Object = new TestClass
        {
            List = new List<int>
                {
                    1, 2, 3, 4
                },
            Tuple = (7, "hello"),
            KVP = new KeyValuePair<int, string>(4, "good"),
            Two = new int[][]
                {
                    new[] { 1, 2, 3, 4 },
                    new[] { 5, 6, 7, 8 }
                },
            Dic = new Dictionary<int, bool>
            {
                [1] = true,
                [2] = false,
                [6] = true
            }
        };

        private readonly SeccsFormatter<BinaryWriter> Seccs = new SeccsFormatter<BinaryWriter>(TypeFormat.GetFromReadAndWrite<BinaryWriter>());
        private readonly MemoryStream SeccsStream = new MemoryStream();
        private readonly BinaryWriter SeccsWriter;

        public SerializeBenchmark()
        {
            SeccsWriter = new BinaryWriter(SeccsStream);
        }

        [Benchmark]
        public void SECCS()
        {
            SeccsStream.Position = 0;
            Seccs.Serialize(SeccsWriter, Object);
        }

        [Benchmark]
        public void NewtonsoftJson()
        {
            JsonConvert.SerializeObject(Object);
        }

        [Benchmark]
        public void MessagePack()
        {
            MessagePackSerializer.Serialize(Object);
        }
    }
}
