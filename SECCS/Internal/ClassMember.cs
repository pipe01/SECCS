using System;
using System.Diagnostics;
using System.Reflection;

namespace SECCS.Internal
{
    [DebuggerDisplay("{Name}")]
    public class ClassMember
    {
        public MemberInfo Member { get; }
        public Type MemberType { get; }
        public string Name => Member.Name;

        public ClassMember(MemberInfo member)
        {
            this.Member = member;
            this.MemberType = member is PropertyInfo p ? p.PropertyType : member is FieldInfo f ? f.FieldType : throw new ArgumentException();
        }

        public bool HasAttribute<T>() where T : Attribute
            => Member.IsDefined(typeof(T));

        public T GetAttribute<T>() where T : Attribute
            => Member.GetCustomAttribute<T>();

        public Type GetTypeOrConcrete()
        {
            var concreteAttr = GetAttribute<ConcreteTypeAttribute>();
            if (concreteAttr != null)
            {
                if (!MemberType.IsAssignableFrom(concreteAttr.Type))
                    throw new Exception($"The concrete type {concreteAttr.Type} is not assignable to {MemberType}");

                return concreteAttr.Type;
            }

            return MemberType;
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

        public static implicit operator ClassMember(PropertyInfo prop) => new ClassMember(prop);
        public static implicit operator ClassMember(FieldInfo prop) => new ClassMember(prop);
    }
}
