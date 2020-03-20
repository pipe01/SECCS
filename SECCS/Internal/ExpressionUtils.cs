using System;
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

        private static Expression Member(Expression inst, ClassMember member)
        {
            if (member.Member is PropertyInfo prop)
                return Property(inst, prop);
            else if (member.Member is FieldInfo field)
                return Field(inst, field);

            throw new Exception("Neither");
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
    }
}
