using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS
{
    using static Expression;

    /// <summary>
    /// Base interface for all type formats. A type format is a class that can generate an <see cref="Expression"/> that
    /// writes an object to a buffer.
    /// </summary>
    public interface ITypeFormat
    {
        bool CanFormat(Type type);

        Expression Serialize(FormatContextWithValue context);
        Expression Deserialize(FormatContext context);
    }

    internal interface IStaticTypeFormat : ITypeFormat
    {
        Type Type { get; }
    }

    /// <summary>
    /// Base class for a type format that handles a single type. See <seealso cref="ITypeFormat"/>
    /// </summary>
    /// <typeparam name="TObject">The type of the object this class handles.</typeparam>
    public abstract class TypeFormat<TObject> : IStaticTypeFormat
    {
        Type IStaticTypeFormat.Type => typeof(TObject);

        bool ITypeFormat.CanFormat(Type type) => type == typeof(TObject);

        public abstract Expression Serialize(FormatContextWithValue context);
        public abstract Expression Deserialize(FormatContext context);
    }

    /// <summary>
    /// Base class for a type format that handles a single type with a known buffer type. See <seealso cref="ITypeFormat"/>
    /// </summary>
    /// <typeparam name="TBuffer">The type of the buffer this class uses</typeparam>
    /// <typeparam name="TObject">The type of the object this class handles</typeparam>
    public abstract class TypedTypeFormat<TBuffer, TObject> : IStaticTypeFormat
    {
        Type IStaticTypeFormat.Type => typeof(TObject);

        bool ITypeFormat.CanFormat(Type type) => type == typeof(TObject);

        Expression ITypeFormat.Serialize(FormatContextWithValue context)
            => Call(Constant(this), nameof(Serialize), null, context.Buffer, Convert(context.Value, typeof(TObject)));

        Expression ITypeFormat.Deserialize(FormatContext context)
            => Call(Constant(this), nameof(Deserialize), null, context.Buffer);

        protected abstract void Serialize(TBuffer buffer, TObject obj);
        protected abstract TObject Deserialize(TBuffer buffer);
    }

    [DebuggerDisplay("{Type.Name}")]
    internal class AutoTypeFormat : IStaticTypeFormat
    {
        private static readonly MethodInfo DebugPrintMethod = typeof(AutoTypeFormat).GetMethod(nameof(DebugPrint), BindingFlags.Static | BindingFlags.NonPublic);

        public Type Type { get; }

        private readonly MethodInfo ReadMethod;
        private readonly MethodInfo WriteMethod;

        public AutoTypeFormat(Type type, MethodInfo readMethod, MethodInfo writeMethod)
        {
            this.Type = type;
            this.ReadMethod = readMethod;
            this.WriteMethod = writeMethod;
        }

        private static void DebugPrint(object obj, string reason, bool read, Type type)
        {
            //Debug.WriteLine($"--{(read ? "Read": "Write")}: ({reason}){(type != null ? $" {type.Name}" : "")} {obj}");
        }

        public bool CanFormat(Type type) => type == Type;

        public Expression Serialize(FormatContextWithValue context)
        {
            var serialExpr = WriteMethod != null
                    ? Call(context.Buffer, WriteMethod, context.Value)
                    : throw new InvalidOperationException($"No serializator method was found for type {Type.FullName}");

            return context.Options.DebugSerialize
                ? (Expression)Block(
                    serialExpr, 
                    Call(DebugPrintMethod, 
                        Convert(context.Value, typeof(object)),
                        Constant(context.Reason, typeof(string)),
                        Constant(false),
                        Constant(null, typeof(Type))))
                : serialExpr;
        }

        public Expression Deserialize(FormatContext context)
        {
            var deserExpr = ReadMethod != null
                    ? Call(context.Buffer, ReadMethod)
                    : throw new InvalidOperationException($"No deserializator method was found for type {Type.FullName}");

            if (context.Options.DebugDeserialize)
            {
                var readVar = Variable(context.DeserializableType);

                return Block(new[] { readVar },
                    Assign(readVar, deserExpr),
                    Call(DebugPrintMethod,
                        Convert(readVar, typeof(object)),
                        Constant(context.Reason, typeof(string)),
                        Constant(true),
                        Constant(context.DeserializableType)),
                    readVar);
            }

            return deserExpr;
        }
    }

    internal class LambdaFormat<TBuffer, TObject> : TypedTypeFormat<TBuffer, TObject>
    {
        private readonly Func<TBuffer, TObject> Deserializer;
        private readonly Action<TBuffer, TObject> Serializer;

        public LambdaFormat(Func<TBuffer, TObject> deserializer, Action<TBuffer, TObject> serializer)
        {
            this.Deserializer = deserializer;
            this.Serializer = serializer;
        }

        protected override TObject Deserialize(TBuffer buffer) => Deserializer(buffer);

        protected override void Serialize(TBuffer buffer, TObject obj) => Serializer(buffer, obj);
    }

    /// <summary>
    /// Contains helper methods for type formats.
    /// </summary>
    public static class TypeFormat
    {
        /// <summary>
        /// Generates type formats from a type's Read() and Write...() methods.
        /// </summary>
        /// <typeparam name="TBuffer">The buffer type</typeparam>
        public static IEnumerable<ITypeFormat> GetFromReadAndWrite<TBuffer>()
        {
            var readMethods = new Dictionary<Type, MethodInfo>();
            var writeMethods = new Dictionary<Type, MethodInfo>();

            foreach (var method in typeof(TBuffer).GetMethods())
            {
                var readAttr = method.GetCustomAttribute<ReadMethodAttribute>();
                var writeAttr = method.GetCustomAttribute<WriteMethodAttribute>();

                if (method.IsPrivate && readAttr == null && writeAttr == null)
                    continue;

                var @params = method.GetParameters();

                if (@params.Length == 0 && (readAttr != null || (method.Name.StartsWith("Read") && method.Name.Substring(4) == method.ReturnType.Name)))
                {
                    var type = readAttr?.ForType ?? method.ReturnType;

                    if (!readMethods.ContainsKey(type))
                        readMethods.Add(type, method);
                }
                else if (@params.Length == 1 && (writeAttr != null || method.Name.StartsWith("Write")) && !writeMethods.ContainsKey(@params[0].ParameterType))
                {
                    writeMethods.Add(@params[0].ParameterType, method);
                }
            }

            var addedTypes = new List<Type>();

            foreach (var readMethod in readMethods)
            {
                writeMethods.TryGetValue(readMethod.Key, out var writeMethod);
                addedTypes.Add(readMethod.Key);

                yield return new AutoTypeFormat(readMethod.Key, readMethod.Value, writeMethod);
            }

            foreach (var writeMethod in writeMethods)
            {
                if (!addedTypes.Contains(writeMethod.Key))
                    yield return new AutoTypeFormat(writeMethod.Key, null, writeMethod.Value);
            }
        }
    }
}
