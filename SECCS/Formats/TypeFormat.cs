using System;
using System.Collections.Generic;

namespace SECCS.Formats
{
    internal class TypeFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        private static readonly IDictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        public bool CanFormat(Type type, FormatOptions options) => typeof(Type).IsAssignableFrom(type);

        public object Read(Type type, IReadFormatContext<T> context)
        {
            string fullName = context.Read<string>("FullName");

            if (!TypeCache.TryGetValue(fullName, out var t))
            {
                TypeCache[fullName] = t = Type.GetType(fullName, throwOnError: true);
            }

            return t;
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            var t = (Type)obj;

            context.Write(t.AssemblyQualifiedName, "FullName");
        }
    }
}
