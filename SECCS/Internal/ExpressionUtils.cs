using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.Internal
{
    using static Expression;

    public delegate void MemberSetterDelegate(object instance, object value);
    public delegate object MemberGetterDelegate(object instance);

    internal static class ExpressionUtils
    {
        private static readonly Cache<(Type, MemberInfo), MemberGetterDelegate> GetterCache = new Cache<(Type, MemberInfo), MemberGetterDelegate>();
        private static readonly IDictionary<Type, Func<object>> CtorCacheNoParams = new Dictionary<Type, Func<object>>();

        private static Expression Member(Expression inst, ClassMember member)
        {
            if (member.Member is PropertyInfo prop)
                return Property(inst, prop);
            else if (member.Member is FieldInfo field)
                return Field(inst, field);

            throw new Exception("Neither");
        }

        public static object New(Type t)
        {
            if (!CtorCacheNoParams.TryGetValue(t, out var ctor))
            {
                CtorCacheNoParams[t] = ctor = Lambda<Func<object>>(Convert(Expression.New(t), typeof(object))).Compile();
            }

            return ctor();
        }

        public static MemberSetterDelegate MemberSetter(Type t, ClassMember member)
        {
            var objParam = Parameter(typeof(object));
            var valueParam = Parameter(typeof(object));

            return Lambda<MemberSetterDelegate>(Assign(Member(Convert(objParam, t), member), Convert(valueParam, member.MemberType)), objParam, valueParam).Compile();
        }

        public static MemberGetterDelegate MemberGetter(Type t, ClassMember member)
        {
            return GetterCache.GetOrCreate((t, member.Member), () =>
            {
                var objParam = Parameter(typeof(object));

                return Lambda<MemberGetterDelegate>(Convert(Member(Convert(objParam, t), member), typeof(object)), objParam).Compile();
            });
        }

        public static Expression ForLoop(Expression length, Func<ParameterExpression, Expression> body)
        {
            var breakLabel = Label("_break");
            var indexVar = Variable(typeof(int));

            return Loop(IfThenElse(
                    LessThan(indexVar, length),
                    body(indexVar),
                    Break(breakLabel)),
                breakLabel);
        }
    }
}
