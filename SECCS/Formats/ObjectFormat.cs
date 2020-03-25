using SECCS.Exceptions;
using SECCS.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.Formats
{
    [FormatPriority(-20)]
    internal class ObjectFormat<T> : IReadFormat<T>, IWriteFormat<T>
    {
        private static readonly IDictionary<Type, Func<object>> NewExpressions = new Dictionary<Type, Func<object>>();
        private static readonly IDictionary<Type, IEnumerable<ClassMember>> ReadableMembers = new Dictionary<Type, IEnumerable<ClassMember>>();
        private static readonly IDictionary<Type, IEnumerable<ClassMember>> WriteableMembers = new Dictionary<Type, IEnumerable<ClassMember>>();

        internal const string NullPath = "@Null";

        public bool CanFormat(Type type, FormatOptions options) => !type.IsPrimitive;

        public object Read(Type type, IReadFormatContext<T> context)
        {
            if (!NewExpressions.TryGetValue(type, out var maker))
                NewExpressions[type] = maker = CreateExpression(type);

            var obj = maker();

            foreach (var member in GetReadableMembers(type))
            {
                object value = context.Read(member.GetTypeOrConcrete(), member.Name);

                var setter = ReflectionUtils.MemberSetter(type, member);

                setter(obj, value);
            }

            return obj;
        }

        public void Write(object obj, IWriteFormatContext<T> context)
        {
            Type t = obj.GetType();

            foreach (var member in GetWriteableMembers(t))
            {
                var getter = ReflectionUtils.MemberGetter(t, member);

                context.Write(getter(obj), member.Name);
            }
        }

        private static IEnumerable<ClassMember> GetReadableMembers(Type t)
        {
            if (!ReadableMembers.TryGetValue(t, out var members))
            {
                ReadableMembers[t] = members = GetMembers(t, true, false).ToArray();
            }

            return members;
        }
        
        private static IEnumerable<ClassMember> GetWriteableMembers(Type t)
        {
            if (!WriteableMembers.TryGetValue(t, out var members))
            {
                WriteableMembers[t] = members = GetMembers(t, false, true).ToArray();
            }

            return members;
        }

        private static IEnumerable<ClassMember> GetMembers(Type t, bool readable, bool writeable)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            foreach (var prop in t.GetProperties(flags))
            {
                if ((!prop.CanRead && readable) || (!prop.CanWrite && writeable) || prop.GetIndexParameters().Length != 0)
                    continue;

                var isPublic = (prop.GetMethod.IsPublic || prop.GetMethod.HasSeccsMember())
                            && (prop.SetMethod.IsPublic || prop.SetMethod.HasSeccsMember());

                var isMember = prop.HasSeccsMember();
                var isIgnored = prop.IsDefined(typeof(SeccsIgnoreAttribute));

                if ((isPublic || isMember) && !isIgnored)
                    yield return prop;
            }

            foreach (var field in t.GetFields(flags))
            {
                if (field.IsInitOnly && readable)
                    continue;

                var isMember = field.IsDefined(typeof(SeccsMemberAttribute));
                var isIgnored = field.IsDefined(typeof(SeccsIgnoreAttribute));

                if ((field.IsPublic || isMember) || isIgnored)
                    yield return field;
            }
        }

        private static Func<object> CreateExpression(Type t)
        {
            Expression expr;

            if (t.IsValueType)
            {
                expr = Expression.Convert(Expression.New(t), typeof(object));
            }
            else
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

                expr = Expression.New(ctor);
            }

            return Expression.Lambda<Func<object>>(expr).Compile();
        }
    }
}
