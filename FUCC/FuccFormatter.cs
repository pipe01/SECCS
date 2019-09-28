using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FUCC
{
    public class FuccFormatter<TBuffer>
    {
        private readonly IDictionary<Type, Func<TBuffer, object>> Deserializers = new Dictionary<Type, Func<TBuffer, object>>();
        private readonly IDictionary<Type, Action<TBuffer, object>> Serializers = new Dictionary<Type, Action<TBuffer, object>>();

        private readonly IDictionary<Type, MethodInfo> ReadMethods = new Dictionary<Type, MethodInfo>();
        private readonly IDictionary<Type, MethodInfo> WriteMethods = new Dictionary<Type, MethodInfo>();

        public FuccFormatter(IEnumerable<MethodInfo> readMethods, IEnumerable<MethodInfo> writeMethods)
        {
            foreach (var item in readMethods)
            {
                if (item.GetParameters().Length != 0)
                    throw new ArgumentException($"Read method '{item.Name}' must have no parameters");

                if (item.ReturnType == typeof(void))
                    throw new ArgumentException($"Read method '{item.Name}' must return a value");

                ReadMethods.Add(item.ReturnType, item);
            }

            foreach (var item in writeMethods)
            {
                var @params = item.GetParameters();

                if (@params.Length != 1)
                    throw new ArgumentException($"Write method '{item.Name}' must have a single parameter");

                WriteMethods.Add(@params[0].ParameterType, item);
            }
        }
    }
}
