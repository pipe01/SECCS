using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FUCC;
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
            BenchmarkRunner.Run<SerializeBenchmark>();
        }
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

        private readonly FuccFormatter<BinaryWriter> Fucc = new FuccFormatter<BinaryWriter>(TypeFormat.GetFromReadAndWrite<BinaryWriter>());
        private readonly MemoryStream FuccStream = new MemoryStream();
        private readonly BinaryWriter FuccWriter;

        public SerializeBenchmark()
        {
            FuccWriter = new BinaryWriter(FuccStream);
        }

        [Benchmark]
        public void FUCC()
        {
            FuccStream.Position = 0;
            Fucc.Serialize(FuccWriter, Object);
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
