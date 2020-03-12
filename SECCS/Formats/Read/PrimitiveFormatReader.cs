﻿using AgileObjects.NetStandardPolyfills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SECCS.Formats.Read
{
    using static Expression;

    [FormatPriority(100)]
    public class PrimitiveFormatReader<TReader> : IReadFormat<TReader>
    {
        private static readonly IDictionary<Type, Func<TReader, object>> ReaderMethods = new Dictionary<Type, Func<TReader, object>>();

        static PrimitiveFormatReader()
        {
            foreach (var method in typeof(TReader).GetPublicInstanceMethods())
            {
                var methodParams = method.GetParameters();

                if (method.Name == "Read" || !method.Name.StartsWith("Read") || methodParams.Length > 0)
                    continue;

                var readerValueType = method.ReturnType;

                if (ReaderMethods.ContainsKey(readerValueType))
                    continue;

                ReaderMethods.Add(readerValueType, CreateMethodAction(method));
            }


            static Func<TReader, object> CreateMethodAction(MethodInfo method)
            {
                var readerParam = Parameter(typeof(TReader));

                Expression callExpr = Call(readerParam, method);

                if (method.ReturnType.IsValueType)
                    callExpr = Convert(callExpr, typeof(object));

                return Lambda<Func<TReader, object>>(callExpr, readerParam).Compile();
            }
        }

        public bool CanFormat(Type type) => ReaderMethods.ContainsKey(type);

        public object Read(Type type, ReadFormatContext<TReader> context)
        {
            return ReaderMethods[type](context.Reader);
        }
    }
}
