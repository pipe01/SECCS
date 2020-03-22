using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SECCS.Formats;
using System.Collections.Generic;

namespace SECCS.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(FormatsBenchmark)
            }).Run(args);
        }
    }

    public class FormatsBenchmark
    {
        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WritePrimitive(bool valueType)
        {
            Singleton<PrimitiveFormatWriter<DummyBuffer>>.Instance.Write(valueType ? (object)123 : "hello", DummyContext.Instance);
        }

        [Benchmark]
        public void WriteTuple()
        {
            Singleton<TupleFormat<DummyBuffer>>.Instance.Write((123, "nice"), DummyContext.Instance);
        }

        [Benchmark]
        public void WriteObject()
        {
            Singleton<ObjectFormat<DummyBuffer>>.Instance.Write(new Class1 { String = "asdasd", Integer = 123 }, DummyContext.Instance);
        }

        [Benchmark]
        public void WriteDictionary()
        {
            Singleton<DictionaryWriteFormat<DummyBuffer>>.Instance.Write(new Dictionary<int, int> { [1] = 2, [3] = 4, [5] = 6 }, DummyContext.Instance);
        }

        [Benchmark]
        public void WriteKeyValuePair()
        {
            Singleton<KeyValuePairFormat<DummyBuffer>>.Instance.Write(new KeyValuePair<int, int>(23, 34), DummyContext.Instance);
        }

        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WriteList(bool valueType)
        {
            Singleton<ListFormat<DummyBuffer>>.Instance.Write(valueType ? (object)new List<int> { 1, 2, 3, 4, 5 } : new List<string> { "a", "b", "c", "d", "e" }, DummyContext.Instance);
        }

        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WriteArray(bool valueType)
        {
            Singleton<ListFormat<DummyBuffer>>.Instance.Write(valueType ? (object)new[] { 1, 2, 3, 4, 5 } : new object[] { "a", "b", "c", "d", "e" }, DummyContext.Instance);
        }
    }

    public static class Singleton<T> where T : new()
    {
        public static readonly T Instance = new T();
    }

    public class DummyContext : IWriteFormatContext<DummyBuffer>
    {
        public static readonly DummyContext Instance = new DummyContext();

        public FormatOptions Options { get; } = new FormatOptions();
        public DummyBuffer Writer { get; } = new DummyBuffer();

        public IWriteFormatContext<DummyBuffer> Write(object obj, string path = "<>", bool nullMark = true)
        {
            return this;
        }
    }

    public class Class1
    {
        public string String;
        public int Integer { get; set; }
    }
}
