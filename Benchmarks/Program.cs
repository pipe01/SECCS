using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SECCS.Formats;
using System;
using System.Collections.Generic;

namespace SECCS.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(WriteFormatsBenchmark),
                typeof(ReadFormatsBenchmark),
            }).Run(args);
        }
    }

    public class WriteFormatsBenchmark
    {
        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WritePrimitive(bool valueType)
        {
            Singleton<PrimitiveFormatWriter<DummyBuffer>>.Instance.Write(valueType ? (object)123 : "hello", DummyWriteContext.Instance);
        }

        [Benchmark]
        public void WriteTuple()
        {
            Singleton<TupleFormat<DummyBuffer>>.Instance.Write((123, "nice"), DummyWriteContext.Instance);
        }

        [Benchmark]
        public void WriteObject()
        {
            Singleton<ObjectFormat<DummyBuffer>>.Instance.Write(new Class1 { String = "asdasd", Integer = 123 }, DummyWriteContext.Instance);
        }

        [Benchmark]
        public void WriteDictionary()
        {
            Singleton<DictionaryWriteFormat<DummyBuffer>>.Instance.Write(new Dictionary<int, int> { [1] = 2, [3] = 4, [5] = 6 }, DummyWriteContext.Instance);
        }

        [Benchmark]
        public void WriteKeyValuePair()
        {
            Singleton<KeyValuePairFormat<DummyBuffer>>.Instance.Write(new KeyValuePair<int, int>(23, 34), DummyWriteContext.Instance);
        }

        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WriteList(bool valueType)
        {
            Singleton<ListFormat<DummyBuffer>>.Instance.Write(valueType ? (object)new List<int> { 1, 2, 3, 4, 5 } : new List<string> { "a", "b", "c", "d", "e" }, DummyWriteContext.Instance);
        }

        [Benchmark]
        [Arguments(true)]
        [Arguments(false)]
        public void WriteArray(bool valueType)
        {
            Singleton<ListFormat<DummyBuffer>>.Instance.Write(valueType ? (object)new[] { 1, 2, 3, 4, 5 } : new object[] { "a", "b", "c", "d", "e" }, DummyWriteContext.Instance);
        }
    }

    public class ReadFormatsBenchmark
    {
        [Benchmark]
        public void ReadPrimitive()
        {
            Singleton<PrimitiveFormatReader<DummyBuffer>>.Instance.Read(typeof(int), DummyReadContext.Instance);
        }

        [Benchmark]
        public void ReadTuple()
        {
            Singleton<TupleFormat<DummyBuffer>>.Instance.Read(typeof((int, string)), DummyReadContext.Instance);
        }

        [Benchmark]
        public void ReadObject()
        {
            Singleton<ObjectFormat<DummyBuffer>>.Instance.Read(typeof(Class1), DummyReadContext.Instance);
        }

        [Benchmark]
        public void ReadArray()
        {
            Singleton<ListFormat<DummyBuffer>>.Instance.Read(typeof(int[]), DummyReadContext.Instance);
        }
    }

    public static class Singleton<T> where T : new()
    {
        public static readonly T Instance = new T();
    }

    public class DummyWriteContext : IWriteFormatContext<DummyBuffer>
    {
        public static readonly DummyWriteContext Instance = new DummyWriteContext();

        public FormatOptions Options { get; } = new FormatOptions();
        public DummyBuffer Writer { get; } = new DummyBuffer();

        public IWriteFormatContext<DummyBuffer> Write(object obj, string path = "<>", bool nullMark = true)
        {
            return this;
        }
    }

    public class DummyReadContext : IReadFormatContext<DummyBuffer>
    {
        public static readonly DummyReadContext Instance = new DummyReadContext();

        public DummyBuffer Reader { get; } = new DummyBuffer();
        public FormatOptions Options { get; } = new FormatOptions();

        public object Read(Type type, PathGetter path = null, bool nullCheck = true)
        {
            if (type == typeof(int))
                return 2;
            else if (type == typeof(string))
                return "";

            return Activator.CreateInstance(type);
        }
    }

    public class Class1
    {
        public string String;
        public int Integer { get; set; }
    }
}
