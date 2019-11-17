using SECCS.Exceptions;
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

        private static ConstructorInfo GetCtor(Type type, Type bufferType)
            => type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { bufferType }, null);

        private static MethodInfo GetMethod(Type type, Type bufferType)
        {
            do
            {
                var method = type.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static, null, new[] { bufferType }, null);

                if (method != null)
                    return method;
            } while ((type = type.BaseType) != null);

            return null;
        }

        public bool CanFormat(Type type)
        {
            var intf = GetInterface(type);

            if (intf != null)
            {
                var bufferType = intf.GetGenericArguments()[0];

                if (GetCtor(type, bufferType) == null)
                {
                    var method = GetMethod(type, bufferType);

                    if (method == null || !type.IsAssignableFrom(method.ReturnType))
                    {
                        throw new InvalidSeccsSerializableException("A class that implements the ISeccsSerializable`1 attribute must contain " +
                             "either a constructor that only takes in the buffer type, or a static method called 'Deserialize' with " +
                             "the same parameter that returns an object of the class' type.\n\nOffending type: " + type.FullName);
                    }
                }

                return true;
            }

            return false;
        }

        public Expression Deserialize(FormatContext context)
        {
            var ctor = GetCtor(context.DeserializableType, context.BufferType);

            if (ctor != null)
                return New(ctor, context.Buffer);

            var method = GetMethod(context.DeserializableType, context.BufferType);

            if (method != null)
                return Call(method, context.Buffer);

            throw new Exception();
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
