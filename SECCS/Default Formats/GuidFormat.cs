using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.DefaultFormats
{
    internal class GuidFormat : IStaticTypeFormat
    {
        public Type Type => typeof(Guid);

        public bool CanFormat(Type type) => type == Type;

        public Expression Deserialize(FormatContext context)
        {
            return Expression.New(
                typeof(Guid).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(byte[]) }, null),
                context.Read<byte[]>());
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            return context.Write<byte[]>(Expression.Call(context.Value, nameof(Guid.ToByteArray), null));
        }
    }
}
