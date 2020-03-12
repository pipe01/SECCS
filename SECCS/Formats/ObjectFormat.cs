using System;
using System.Collections.Generic;
using System.Reflection;

namespace SECCS.Formats
{
    [FormatPriority(-20)]
    public class ObjectFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type) => !type.IsPrimitive;

        public object Read(Type type, ReadFormatContext<T> context)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var item in GetProperties(type))
            {
                var value = context.Deserialize(item.PropertyType, item.Name);
                item.SetValue(obj, value);
            }

            return obj;
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            Type t = obj.GetType();

            foreach (var item in GetProperties(t))
            {
                if (item.CanRead)
                    context.Serialize(item.GetValue(obj), item.Name);
            }
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type t)
        {
            foreach (var prop in t.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead && !prop.CanWrite)
                    continue;

                if ((prop.GetMethod.IsPublic && prop.SetMethod.IsPublic) || prop.GetCustomAttribute<SeccsMemberAttribute>() != null)
                    yield return prop;
            }
        }
    }
}
