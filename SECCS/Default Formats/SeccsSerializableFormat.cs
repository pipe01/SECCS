using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.DefaultFormats
{
    using static Expression;

    internal class SeccsSerializableFormat : ITypeFormat
    {
        private Type GetInterface(Type type) => Array.Find(type.GetInterfaces(), o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(ISeccsSerializable<>));

        public bool CanFormat(Type type)
        {
            var intf = GetInterface(type);
            if (intf != null)
            {
                if (type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { intf.GetGenericArguments()[0] }, null) == null)
                    throw new Exception($"Type {type.FullName} implements ISeccsSerializable`1 but doesn't have a suitable constructor");

                return true;
            }

            return false;
        }

        public Expression Deserialize(FormatContext context)
        {
            var ctor = context.DeserializableType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { context.BufferType }, null);

            return New(ctor, context.Buffer);
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var intf = GetSerializableInterface(context.Type, context.BufferType);

            return Call(Convert(context.Value, intf), intf.GetMethod("Serialize"), context.Buffer);
        }

        private static Type GetSerializableInterface(Type type, Type bufferType)
        {
            return type
                .GetInterfaces()
                .Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(ISeccsSerializable<>))
                .FirstOrDefault(o => o.GetGenericArguments()[0] == bufferType);
        }
    }
}
