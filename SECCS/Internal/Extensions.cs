using System;
using System.Reflection;

namespace SECCS.Internal
{
    internal static class Extensions
    {
        public static MethodInfo GetMethodInAnyInterface(this Type type, string methodName, Type[] paramTypes)
        {
            var method = type.GetMethod(methodName, paramTypes);

            if (method != null)
                return method;

            foreach (var item in type.GetInterfaces())
            {
                method = item.GetMethod(methodName, paramTypes);

                if (method != null)
                    return method;
            }

            return null;
        }
    }
}
