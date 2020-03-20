using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.Formats
{
    using static Expression;

    [FormatPriority(100)]
    internal class PrimitiveFormatWriter<TWriter> : IWriteFormat<TWriter>
    {
        private static readonly IDictionary<Type, Action<TWriter, object>> WriterMethods = new Dictionary<Type, Action<TWriter, object>>();

        static PrimitiveFormatWriter()
        {
            foreach (var method in typeof(TWriter).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var methodParams = method.GetParameters();

                if (!method.Name.StartsWith("Write") || methodParams.Length != 1)
                    continue;

                var writerValueType = methodParams[0].ParameterType;

                WriterMethods.Add(writerValueType, CreateMethodAction(method, writerValueType));
            }

            Action<TWriter, object> CreateMethodAction(MethodInfo method, Type writerValueType)
            {
                var writerParam = Parameter(typeof(TWriter));
                var objParam = Parameter(typeof(object));

                var callExpr = Call(writerParam, method, Convert(objParam, writerValueType));

                return Lambda<Action<TWriter, object>>(callExpr, writerParam, objParam).Compile();
            }
        }

        public bool CanFormat(Type type, FormatOptions options) => WriterMethods.ContainsKey(type);

        public void Write(object obj, IWriteFormatContext<TWriter> context)
        {
            WriterMethods[obj.GetType()](context.Writer, obj);
        }
    }
}
