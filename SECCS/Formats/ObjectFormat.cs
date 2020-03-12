using System;

namespace SECCS.Formats
{
    [FormatPriority(-20)]
    public class ObjectFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        public bool CanFormat(Type type) => !type.IsPrimitive;

        public object Read(Type type, ReadFormatContext<T> context)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var item in type.GetProperties())
            {
                var value = context.Deserialize(item.PropertyType, item.Name);
                item.SetValue(obj, value);
            }

            return obj;
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            Type t = obj.GetType();

            foreach (var item in t.GetProperties())
            {
                if (item.CanRead)
                    context.Serialize(item.GetValue(obj), item.Name);
            }
        }
    }
}
