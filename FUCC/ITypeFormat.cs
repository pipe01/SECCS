using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FUCC
{
    public interface ITypeFormat
    {
        bool CanFormat(Type type);

        Expression Serialize(Expression buffer, Expression value);
        Expression Deserialize(Expression buffer);
    }

    public abstract class TypeFormat<TObject> : ITypeFormat
    {
        bool ITypeFormat.CanFormat(Type type) => type == typeof(TObject);

        public abstract Expression Serialize(Expression buffer, Expression value);
        public abstract Expression Deserialize(Expression buffer);
    }

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

        public Expression Serialize(Expression buffer, Expression value) => Expression.Call(buffer, WriteMethod, value);

        public Expression Deserialize(Expression buffer) => Expression.Call(buffer, ReadMethod);
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

                if (readAttr != null || (method.Name.StartsWith("Read") && method.Name.Substring(4) == method.ReturnType.Name))
                {
                    readMethods.Add(readAttr?.ForType ?? method.ReturnType, method);
                }
                else if (writeAttr != null || method.Name.StartsWith("Write"))
                {
                    var @params = method.GetParameters();

                    if (@params.Length == 1)
                    {
                        writeMethods.Add(@params[0].ParameterType, method);
                    }
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
