using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace SECCS.Internal
{
    [DebuggerDisplay("{Name}")]
    public class ClassMember
    {
        private static readonly IDictionary<MemberInfo, ClassMember> MemberCache = new Dictionary<MemberInfo, ClassMember>();

        public MemberInfo Member { get; }
        public Type MemberType { get; }
        public string Name => Member.Name;

        private readonly Lazy<Type> ConcreteType;

        public ClassMember(MemberInfo member)
        {
            this.Member = member;
            this.MemberType = member is PropertyInfo p ? p.PropertyType : member is FieldInfo f ? f.FieldType : throw new ArgumentException();

            this.ConcreteType = new Lazy<Type>(() =>
            {
                var concreteAttr = GetAttribute<ConcreteTypeAttribute>();
                if (concreteAttr != null)
                {
                    if (!MemberType.IsAssignableFrom(concreteAttr.Type))
                        throw new Exception($"The concrete type {concreteAttr.Type} is not assignable to {MemberType}");

                    return concreteAttr.Type;
                }

                return null;
            });
        }

        public bool HasAttribute<T>() where T : Attribute
            => Member.IsDefined(typeof(T));

        public T GetAttribute<T>() where T : Attribute
            => Member.GetCustomAttribute<T>();

        public Type GetTypeOrConcrete()
        {
            return ConcreteType.Value ?? MemberType;
        }

        public object GetValue(object obj)
        {
            if (Member is PropertyInfo prop)
                return prop.GetValue(obj);
            else if (Member is FieldInfo field)
                return field.GetValue(obj);

            throw new Exception("Not a field or a property");
        }

        public void SetValue(object obj, object value)
        {
            if (Member is PropertyInfo prop)
                prop.SetValue(obj, value);
            else if (Member is FieldInfo field)
                field.SetValue(obj, value);
            else
                throw new Exception("Not a field or a property");
        }

        public static implicit operator ClassMember(MemberInfo member) => MemberCache.TryGetValue(member, out var m) ? m : MemberCache[member] = new ClassMember(member);
    }
}
