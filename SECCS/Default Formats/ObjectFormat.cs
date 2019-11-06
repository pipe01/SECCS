using SECCS.Attributes;
using SECCS.Exceptions;
using SECCS.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace SECCS.DefaultFormats
{
    using static Expression;

    [Priority(-999)]
    public class ObjectFormat : ITypeFormat
    {
        public bool CanFormat(Type type) => !type.IsPrimitive;

        public Expression Deserialize(FormatContext context)
        {
            var objType = context.DeserializableType ?? context.Type;
            var objVar = Variable(objType, "_obj");
            var exprs = new List<Expression>();

            var members = GetMembers(objType).ToArray();

            var seccsCtor = objType.GetConstructors().SingleOrDefault(o => o.IsDefined(typeof(SeccsConstructorAttribute)));

            if (seccsCtor != null)
            {
                if (seccsCtor.GetParameters().Length != members.Length)
                    throw new InvalidConstructorException($"SECCS constructor for {objType.FullName}'s parameter count must be equal to the number of serializable members in the class");

                var ctorParams = new Queue<ParameterInfo>(seccsCtor.GetParameters());
                var memberList = new List<ClassMember>(members);
                var paramExprs = new List<Expression>();

                while (ctorParams.Count > 0)
                {
                    var param = ctorParams.Dequeue();
                    var member = memberList.Find(o => o.MemberType == param.ParameterType);

                    if (member != null)
                    {
                        memberList.Remove(member);
                        paramExprs.Add(GetExpressionForField(member));
                    }
                    else
                    {
                        throw new InvalidConstructorException($"Mismatched parameter: {param.Name}");
                    }
                }

                exprs.Add(Assign(objVar, New(seccsCtor, paramExprs)));
            }
            else
            {
                exprs.Add(Assign(objVar, New(objType)));

                foreach (var field in members)
                {
                    exprs.Add(GetExpressionForField(field));
                }
            }

            return Block(new[] { objVar }, new[] { NullCheck(objType, objVar, Block(exprs)), objVar });

            Expression GetExpressionForField(ClassMember field)
            {
                if (field.MemberType.IsInterface)
                {
                    if (!field.Member.IsDefined(typeof(ConcreteTypeAttribute)))
                        throw new Exception("Fields of interface types must be decorated with ConcreteTypeAttribute");
                    else if (!field.MemberType.IsAssignableFrom(field.GetConcreteType()))
                        throw new Exception($"The specified concrete type for field '{field.Name}' doesn't implement the field type ({field.MemberType.Name})");
                }

                var format = context.Formats.Get(field.MemberType);

                if (format == null)
                    throw new Exception($"Format not found for '{objType.Name}.{field.Name}'");

                var readExpr = Assign(PropertyOrField(objVar, field.Name), context.Read(field.MemberType));

                return NullCheck(field.MemberType, PropertyOrField(objVar, field.Name), readExpr);
            }

            Expression NullCheck(Type type, Expression value, Expression readExpr)
            {
                if (!type.IsValueType)
                {
                    return IfThenElse(
                        Equal(context.Read(typeof(byte)), Constant((byte)0)),
                        Assign(value, Constant(null, type)),
                        readExpr);
                }
                else
                {
                    return readExpr;
                }
            }
        }

        public Expression Serialize(FormatContextWithValue context)
        {
            var convertedObj = Convert(context.Value, context.Type);
            var exprs = new List<Expression>();

            foreach (var field in GetMembers(context.Type))
            {
                var format = context.Formats.Get(field.MemberType);

                if (format == null)
                    throw new Exception($"Format not found for '{context.Type.Name}.{field.Name}'");

                var fieldExpr = PropertyOrField(convertedObj, field.Name);

                exprs.Add(NullCheck(field.MemberType, fieldExpr, context.Write(field.MemberType, fieldExpr)));
            }

            return NullCheck(context.Type, context.Value, Block(exprs));

            Expression NullCheck(Type type, Expression value, Expression writeExpr)
            {
                if (!type.IsValueType)
                {
                    return IfThenElse(
                        Equal(value, Constant(null)),
                        context.Write(typeof(byte), Constant((byte)0)),
                        Block(new[]
                        {
                            context.Write(typeof(byte), Constant((byte)1)),
                            writeExpr
                        }));
                }
                else
                {
                    return writeExpr;
                }
            }
        }


        private static IEnumerable<ClassMember> GetMembers(Type t)
        {
            var members = t.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(o => !o.IsInitOnly).Select(o => new ClassMember(o)).Concat(
                          t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(o => o.CanWrite && o.CanRead).Select(o => new ClassMember(o)))
                    .Where(o => !o.Member.IsDefined(typeof(IgnoreDataMemberAttribute), false));

            var optionsAttr = t.GetCustomAttribute<SeccsObjectAttribute>();

            if (optionsAttr == null || optionsAttr.MemberSerializing == SeccsMemberSerializing.OptOut)
                return members.Where(o => !o.Member.IsDefined(typeof(SeccsIgnoreAttribute)));
            else
                return members.Where(o => o.Member.IsDefined(typeof(SeccsMemberAttribute)));
        }

    }
}
