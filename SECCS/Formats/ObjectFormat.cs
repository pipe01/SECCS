using SECCS.Internal;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.Formats
{
    [FormatPriority(-20)]
    internal class ObjectFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        private static readonly IDictionary<Type, Func<object>> NewExpressions = new Dictionary<Type, Func<object>>();

        public bool CanFormat(Type type) => !type.IsPrimitive;

        public object Read(Type type, ReadFormatContext<T> context)
        {
            if (!NewExpressions.TryGetValue(type, out var maker))
                NewExpressions[type] = maker = CreateExpression(type);

            var obj = maker();

            foreach (var prop in GetMembers(type))
            {
                var value = context.Read(prop.GetTypeOrConcrete(), prop.Name);
                prop.SetValue(obj, value);
            }

            return obj;
        }

        public void Write(object obj, WriteFormatContext<T> context)
        {
            Type t = obj.GetType();

            foreach (var item in GetMembers(t))
            {
                context.Write(item.GetValue(obj), item.Name);
            }
        }

        private static IEnumerable<ClassMember> GetMembers(Type t)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            foreach (var prop in t.GetProperties(flags))
            {
                if (!prop.CanRead && !prop.CanWrite)
                    continue;

                var isPublic = prop.GetMethod.IsPublic && prop.SetMethod.IsPublic;
                var isMember = prop.IsDefined(typeof(SeccsMemberAttribute));
                var isIgnored = prop.IsDefined(typeof(SeccsIgnoreAttribute));

                if ((isPublic || isMember) && !isIgnored)
                    yield return prop;
            }

            foreach (var field in t.GetFields(flags))
            {
                if (field.IsInitOnly)
                    continue;

                var isMember = field.IsDefined(typeof(SeccsMemberAttribute));
                var isIgnored = field.IsDefined(typeof(SeccsIgnoreAttribute));

                if ((field.IsPublic || isMember) || isIgnored)
                    yield return field;
            }
        }

        private static Func<object> CreateExpression(Type t)
        {
            ConstructorInfo ctor = null;

            foreach (var item in t.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (item.IsDefined(typeof(SeccsConstructorAttribute)))
                {
                    ctor = item;
                    break;
                }
                else if (item.IsPublic && item.GetParameters().Length == 0)
                {
                    ctor = item;
                }
            }

            if (ctor == null)
            {
                throw new MissingMemberException($"No public parameterless constructor found for type {t.FullName}");
            }

            return Expression.Lambda<Func<object>>(Expression.New(ctor)).Compile();
        }
    }
}
