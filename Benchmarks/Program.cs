using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SECCS.Formats.MiscTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SECCS.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(SeccsBenchmark)
            }).Run(args);
        }
    }

    public class SeccsBenchmark
    {
        private readonly SeccsWriter<DummyBuffer> Writer = new SeccsWriter<DummyBuffer>();
        private readonly DummyBuffer Buffer = new DummyBuffer();

        [Benchmark]
        public void WriteString()
        {
            Writer.Serialize(Buffer, "hello");
        }
        
        [Benchmark]
        public void WriteInteger()
        {
            Writer.Serialize(Buffer, 123);
        }
        
        [Benchmark]
        public void WriteTuple()
        {
            Writer.Serialize(Buffer, (123, "nice"));
        }
        
        [Benchmark]
        public void WriteObject()
        {
            Writer.Serialize(Buffer, new Class1 { String = "asdasd", Integer = 123 });
        }
        
        [Benchmark]
        public void WriteDictionary()
        {
            Writer.Serialize(Buffer, new Dictionary<int, int> { [1] = 2, [3] = 4, [5] = 6 });
        }
        
        [Benchmark]
        public void WriteKeyValuePair()
        {
            Writer.Serialize(Buffer, new KeyValuePair<int, int>(23, 34));
        }
        
        [Benchmark]
        public void WriteList()
        {
            Writer.Serialize(Buffer, new List<int> { 1, 2, 3, 4, 5 });
        }
    }

    public class Class1
    {
        public string String;
        public int Integer { get; set; }
    }
}
