using FUCC;
using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace Tester
{
    public class Nested
    {
        public string Str;
        public int Hello;
    }

    public class Test
    {
        public Nested Nested;
        public List<int> List;
        public (int, string) Tuple;
        public KeyValuePair<int, string> KVP;
        public int[][] Two;
        public Dictionary<int, bool> Dic;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var f = new FuccFormatter<NetBuffer>(TypeFormat.GetFromReadAndWrite<NetBuffer>());
            var buffer = new NetBuffer();
            f.Serialize(buffer, new Test
            {
                Nested = new Nested
                {
                    Str = "foobar",
                    Hello = 5
                },
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
            });

            buffer.Position = 0;
            var obj = f.Deserialize<Test>(buffer);
        }
    }
}
