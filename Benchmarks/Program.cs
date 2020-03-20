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
        [ArgumentsSource(nameof(Arguments))]
        public void GuidFormatWrite(object value)
        {
            Writer.Serialize(Buffer, value);
        }

        public IEnumerable<object> Arguments()
        {
            return new object[]
            {
                "hello",
                123,
                (123, "nice"),
            };
        }
    }
}
