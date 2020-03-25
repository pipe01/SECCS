using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SECCS.Internal
{
    using static Expression;

    public delegate void MemberSetterDelegate(object instance, object value);
    public delegate object MemberGetterDelegate(object instance);

    internal static class ReflectionUtils
    {
        private static readonly IDictionary<MemberInfo, MemberGetterDelegate> GetterCache = new Dictionary<MemberInfo, MemberGetterDelegate>();
        private static readonly IDictionary<MemberInfo, MemberSetterDelegate> SetterCache = new Dictionary<MemberInfo, MemberSetterDelegate>();
        private static readonly IDictionary<Type, Func<object>> CtorCacheNoParams = new Dictionary<Type, Func<object>>();
        private static readonly IDictionary<Type, Func<object[], object>> CtorCacheParams = new Dictionary<Type, Func<object[], object>>();
        private static readonly IDictionary<Type, Type[]> GenericParamsCache = new Dictionary<Type, Type[]>();

        public static object New(Type t)
        {
            if (!CtorCacheNoParams.TryGetValue(t, out var ctor))
            {
                CtorCacheNoParams[t] = ctor = Lambda<Func<object>>(Convert(Expression.New(t), typeof(object))).Compile();
            }

            return ctor();
        }
        
        public static object New(Type t, params object[] args)
        {
            if (!CtorCacheParams.TryGetValue(t, out var ctorFunc))
            {
                var argTypes = args.Select(o => o.GetType()).ToArray();
                var ctor = t.GetConstructor(argTypes);
                if (ctor == null)
                    throw new MissingMemberException("Constructor not found");

                var argsParam = Parameter(typeof(object[]));

                CtorCacheParams[t] = ctorFunc = Lambda<Func<object[], object>>(
                    Convert(
                        Expression.New(
                            ctor,
                            args.Select((_, i) => Convert(ArrayAccess(argsParam, Constant(i)), argTypes[i]))),
                        typeof(object)), argsParam).Compile();
            }

            return ctorFunc(args);
        }

        public static Type[] GetGenericParams(Type t)
        {
            if (!GenericParamsCache.TryGetValue(t, out var p))
            {
                GenericParamsCache[t] = p = t.GetGenericArguments();
            }

            return p;
        }

        public static MemberSetterDelegate MemberSetter(Type t, ClassMember member)
        {
            if (!SetterCache.TryGetValue(member.Member, out var setter))
            {
                var objParam = Parameter(typeof(object));
                var valueParam = Parameter(typeof(object));

                SetterCache[member.Member] = setter = Lambda<MemberSetterDelegate>(Assign(Member(Convert(objParam, t), member), Convert(valueParam, member.MemberType)), objParam, valueParam).Compile();
            }

            return setter;
        }

        public static MemberGetterDelegate MemberGetter(Type t, ClassMember member)
        {
            if (!GetterCache.TryGetValue(member.Member, out var getter))
            {
                var objParam = Parameter(typeof(object));

                GetterCache[member.Member] = getter = Lambda<MemberGetterDelegate>(Convert(Member(Convert(objParam, t), member), typeof(object)), objParam).Compile();
            }

            return getter;
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

        private static Expression Member(Expression inst, ClassMember member)
        {
            if (member.Member is PropertyInfo prop)
                return Property(inst, prop);
            else if (member.Member is FieldInfo field)
                return Field(inst, field);

            throw new Exception("Neither");
        }
    }
}
