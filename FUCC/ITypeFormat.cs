using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace FUCC
{
    public interface ITypeFormat
    {
        bool CanFormat(Type type);

        Expression Serialize(FormatContextWithValue context);
        Expression Deserialize(FormatContext context);
    }

    internal interface IStaticTypeFormat
    {
        Type Type { get; }
    }

    public abstract class TypeFormat<TObject> : ITypeFormat, IStaticTypeFormat
    {
        Type IStaticTypeFormat.Type => typeof(TObject);

        bool ITypeFormat.CanFormat(Type type) => type == typeof(TObject);

        public abstract Expression Serialize(FormatContextWithValue context);
        public abstract Expression Deserialize(FormatContext context);
    }

    public abstract class TypedTypeFormat<TBuffer, TObject> : ITypeFormat, IStaticTypeFormat
    {
        Type IStaticTypeFormat.Type => typeof(TObject);

        bool ITypeFormat.CanFormat(Type type) => type == typeof(TObject);

        Expression ITypeFormat.Serialize(FormatContextWithValue context)
            => Expression.Call(Expression.Constant(this), nameof(Serialize), null, context.Buffer, context.Value);

        Expression ITypeFormat.Deserialize(FormatContext context)
            => Expression.Call(Expression.Constant(this), nameof(Deserialize), null, context.Buffer);

        protected abstract void Serialize(TBuffer buffer, TObject obj);
        protected abstract TObject Deserialize(TBuffer buffer);
    }

    [DebuggerDisplay("{Type.Name}")]
    internal class AutoTypeFormat : ITypeFormat
    {
        private readonly Type Type;
        private readonly MethodInfo ReadMethod;
        private readonly MethodInfo WriteMethod;

        public AutoTypeFormat(Type type, MethodInfo readMethod, MethodInfo writeMethod)
        {
            this.Type = type;
            this.ReadMethod = readMethod;
            this.WriteMethod = writeMethod;
        }

        public bool CanFormat(Type type) => type == Type;

        public Expression Serialize(FormatContextWithValue context)
            => Expression.Call(context.Buffer, WriteMethod, context.Value);

        public Expression Deserialize(FormatContext context)
            => Expression.Call(context.Buffer, ReadMethod);
    }

    public static class TypeFormat
    {
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

            foreach (var readMethod in readMethods)
            {
                if (writeMethods.TryGetValue(readMethod.Key, out var writeMethod))
                    yield return new AutoTypeFormat(readMethod.Key, readMethod.Value, writeMethod);
            }
        }
    }
}
