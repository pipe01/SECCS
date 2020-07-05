using System;
using System.Collections.Generic;
using System.Reflection;

namespace SECCS.Formats
{
    internal class TypeFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type, FormatOptions options) => typeof(Type).IsAssignableFrom(type);

        const string TypeNamePath = "TypeName";

        public object Read(Type type, IReadFormatContext<T> context)
        {
            string typeName = context.Read<string>(TypeNamePath);
            return ParseType(typeName);
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            var type = (Type)obj;
            context.Write(SerializeType(type), TypeNamePath);
        }


        // We don't use the assembly qualified name because some programs use different assemblies for the same type, and we want
        // those programs to be able to communicate about those types.
        // For instance, in a .NET Core standalone app (i.e. the Logic World server), System.Int32 is in System.Private.CoreLib.dll.
        // However in a Unity project (i.e. the Logic World client), System.Int32 is in mscorlib.dll.

        // This code is based on the type-parsing code from SUCC.

        private static Dictionary<string, Type> TypeCache { get; } = new Dictionary<string, Type>();
        private static Type ParseType(string typeName)
        {
            if (TypeCache.TryGetValue(typeName, out Type type))
                return type;

            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = ass.GetType(typeName);

                if (type != null)
                {
                    TypeCache.Add(typeName, type);
                    return type;
                }
            }

            throw new Exception($"Cannot parse text {typeName} as System.Type");
        }

        private static string SerializeType(Type type)
            => type.FullName;
    }
}
