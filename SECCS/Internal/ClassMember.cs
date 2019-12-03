using System;
using System.Reflection;

namespace SECCS.Internal
{
    internal class ClassMember
    {
        public MemberInfo Member { get; }
        public Type MemberType { get; }
        public string Name => Member.Name;

        public ClassMember(MemberInfo member)
        {
            this.Member = member;
            this.MemberType = member is PropertyInfo p ? p.PropertyType : member is FieldInfo f ? f.FieldType : throw new ArgumentException();
        }

        public bool HasAttribute<T>() => Member.IsDefined(typeof(T));

        public Type GetConcreteType() => Member.GetCustomAttribute<ConcreteTypeAttribute>()?.Type;

        public static implicit operator ClassMember(PropertyInfo prop) => new ClassMember(prop);
        public static implicit operator ClassMember(FieldInfo prop) => new ClassMember(prop);
    }
}
