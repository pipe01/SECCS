using System;

#if NET45 || NETSTANDARD2_0
using FormatType = System.String;
#else
using FormatType = System.ReadOnlySpan<char>;
#endif

namespace SECCS
{
    public readonly ref struct PathGetter
    {
        public FormatType Format { get; }
        public object Argument { get; }

        public string Path => string.Format(
#if NET45 || NETSTANDARD2_0
            Format
#else
            new string(Format)
#endif
            , Argument);

        public PathGetter(FormatType format, object arg)
        {
            this.Format = format;
            this.Argument = arg;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            return Format.GetHashCode() * (Argument?.GetHashCode() ?? 1);
        }

        public static bool operator ==(PathGetter a, PathGetter b)
        {
            return a.Path == b.Path;
        }

        public static bool operator !=(PathGetter a, PathGetter b)
        {
            return a.Path != b.Path;
        }

        public static implicit operator PathGetter(string str)
        {
            return new PathGetter(str,
#if NET45
                new object[0]
#else
                Array.Empty<object>()
#endif
                );
        }

        public static PathGetter Index(int i) => new PathGetter("[{0}]", i);
    }
}
