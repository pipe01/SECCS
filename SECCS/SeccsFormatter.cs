using SECCS.Attributes;
using SECCS.DefaultFormats;
using SECCS.Exceptions;
using SECCS.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace SECCS
{
    using static Expression;

    public class SeccsFormatter<TBuffer>
    {
        private const byte Magic = 243;
        private const byte MagicWithSignature = 244;

        private static readonly ITypeFormat[] DefaultFormats;

        /// <summary>
        /// Collection of registered type formats.
        /// </summary>
        public TypeFormatCollection<TBuffer> Formats { get; }

        private readonly IDictionary<Type, Func<TBuffer, object>> Deserializers = new Dictionary<Type, Func<TBuffer, object>>();
        private readonly IDictionary<Type, Action<TBuffer, object>> Serializers = new Dictionary<Type, Action<TBuffer, object>>();

        private readonly SeccsOptions Options;

        static SeccsFormatter()
        {
            DefaultFormats = typeof(ITypeFormat).Assembly
                .GetTypes()
                .Where(o => typeof(ITypeFormat).IsAssignableFrom(o) && o.Namespace == typeof(ValueTupleFormat).Namespace)
                .OrderByDescending(o => o.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
                .Select(o => (ITypeFormat)Activator.CreateInstance(o))
                .ToArray();
        }

        /// <summary>
        /// Instantiates a <see cref="SeccsFormatter{TBuffer}"/> with no additional formatters and default options.
        /// </summary>
        /// <param name="options">The options object, or null for default</param>
        public SeccsFormatter(SeccsOptions options = null) : this(Enumerable.Empty<ITypeFormat>(), options)
        {
        }

        /// <summary>
        /// Instantiates a <see cref="SeccsFormatter{TBuffer}"/> with additional formatters and default options.
        /// </summary>
        /// <param name="formats">The additional formatters to be used</param>
        /// <param name="options">The options object, or null for default</param>
        public SeccsFormatter(IEnumerable<ITypeFormat> formats, SeccsOptions options = null)
        {
            this.Formats = new TypeFormatCollection<TBuffer>();
            this.Options = options ?? new SeccsOptions();

            Formats.Register(DefaultFormats);
            Formats.Register(formats);
        }

        private static IEnumerable<ClassMember> GetMembers(Type t)
        {
            var members = t.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(o => new ClassMember(o)).Concat(
                          t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(o => new ClassMember(o)))
                    .Where(o => !o.Member.IsDefined(typeof(IgnoreDataMemberAttribute), false));

            var optionsAttr = t.GetCustomAttribute<SeccsObjectAttribute>();

            if (optionsAttr == null || optionsAttr.MemberSerializing == SeccsMemberSerializing.OptOut)
                return members.Where(o => !o.Member.IsDefined(typeof(SeccsIgnoreAttribute)));
            else
                return members.Where(o => o.Member.IsDefined(typeof(SeccsMemberAttribute)));
        }

        /// <summary>
        /// Serializes <paramref name="obj"/> into <typeparamref name="TBuffer"/> using type formats.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="buffer">The buffer to serialize the object into</param>
        /// <param name="obj">The object to serialize</param>
        public void Serialize<T>(TBuffer buffer, T obj)
            => Serialize(buffer, obj, typeof(T));

        /// <summary>
        /// Serializes an <paramref name="obj"/> of type <paramref name="type"/> into <paramref name="buffer"/>
        /// </summary>
        /// <param name="buffer">The buffer to serialize the object into</param>
        /// <param name="obj">The object to serialize</param>
        /// <param name="type">The object's type, or null to get it from <paramref name="obj"/></param>
        public void Serialize(TBuffer buffer, object obj, Type type = null)
        {
            type = type ?? obj?.GetType() ?? throw new ArgumentNullException("obj is null and no type has been specified");

            if (!Serializers.TryGetValue(type, out var ser))
            {
                var bufferParam = Parameter(typeof(TBuffer), "_buffer");
                var objParam = Parameter(typeof(object), "_obj");
                var exprs = new List<Expression>();

                if (Options.WriteHeader)
                {
                    //Write 243, or 244 if Options.WriteStructureSignature is on
                    exprs.Add(Formats.Get(typeof(byte)).Serialize(new FormatContextWithValue(Formats, typeof(byte), typeof(TBuffer), bufferParam, Constant((byte)(Options.WriteStructureSignature ? MagicWithSignature : Magic)))));

                    if (Options.WriteStructureSignature)
                    {
                        var hash = ClassSignature.Get(type);
                        exprs.Add(Formats.Get(typeof(string)).Serialize(new FormatContextWithValue(Formats, typeof(string), typeof(TBuffer), bufferParam, Constant(hash))));
                    }
                }

                exprs.AddRange(GetBlock(objParam, bufferParam, type));

                Serializers[type] = ser = Lambda<Action<TBuffer, object>>(Block(exprs), bufferParam, objParam).Compile();
            }

            ser(buffer, obj);

            IEnumerable<Expression> GetBlock(Expression objExpr, Expression bufferExpr, Type objType)
            {
                var convertedObj = Convert(objExpr, objType);

                foreach (var field in GetMembers(objType))
                {
                    var format = Formats.Get(field.MemberType);

                    if (format == null)
                    {
                        if (Options.SerializeUnknownTypes)
                            yield return Block(GetBlock(PropertyOrField(convertedObj, field.Name), bufferExpr, field.MemberType));
                        else
                            throw new Exception($"Format not found for '{type.Name}.{field.Name}'");
                    }
                    else
                    {
                        yield return format.Serialize(new FormatContextWithValue(
                            Formats, field.MemberType, typeof(TBuffer), bufferExpr, PropertyOrField(convertedObj, field.Name), field.GetConcreteType()));
                    }
                }
            }
        }

        /// <summary>
        /// Reads a <typeparamref name="T"/> from a <paramref name="buffer"/> of type <typeparamref name="TBuffer"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to read</typeparam>
        /// <param name="buffer">The buffer to read from</param>
        /// <returns></returns>
        public T Deserialize<T>(TBuffer buffer)
            => (T)Deserialize(buffer, typeof(T));

        /// <summary>
        /// Reads an object of type <paramref name="type"/> from <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to read from</param>
        /// <param name="type">The type of the object to read</param>
        public object Deserialize(TBuffer buffer, Type type)
        {
            if (!Deserializers.TryGetValue(type, out var des))
            {
                var exprs = new List<Expression>();
                var bufferParam = Parameter(typeof(TBuffer), "_buffer");
                var objVar = Variable(typeof(object), "_obj");

                if (Options.CheckHeader)
                {
                    var magicVar = Variable(typeof(byte), "_magic");
                    var hash = ClassSignature.Get(type);

                    /*
                     byte magic = Read<byte>();
                     if (magic == 244)
                     {
                         if (Read<string>() != "HASH")
                         {
                             if (Options.CheckStructureSignature)
                                 throw new Exception();
                         }
                     }
                     else if (magic != 243)
                     {
                         throw new Exception();
                     }
                     */
                    exprs.Add(Block(new[] { magicVar },
                        Assign(magicVar, Read<byte>()),
                        IfThenElse(
                            Equal(magicVar, Constant(MagicWithSignature)),
                            IfThen(NotEqual(Read<string>(), Constant(hash)), Options.CheckStructureSignature ? InvalidHeaderException.Throw("Class structure signature mismatch") : Block()),
                            IfThen(NotEqual(magicVar, Constant(Magic)), InvalidHeaderException.Throw("Invalid magic number")))));
                }

                exprs.AddRange(GetBlock(objVar, bufferParam, type));
                exprs.Add(objVar);

                Deserializers[type] = des = Lambda<Func<TBuffer, object>>(Block(new[] { objVar }, exprs), bufferParam).Compile();

                Expression Read<T>() => Formats.Get(typeof(T))?.Deserialize(new FormatContext(Formats, typeof(T), typeof(TBuffer), bufferParam)) ?? throw new InvalidOperationException("Cannot deserialize type " + typeof(T).FullName);
            }

            return des(buffer);

            IEnumerable<Expression> GetBlock(Expression objExpr, Expression bufferExpr, Type objType)
            {
                var convertedObj = Convert(objExpr, objType);
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

                    yield return Assign(objExpr, New(seccsCtor, paramExprs));
                }
                else
                {
                    yield return Assign(objExpr, New(objType));

                    foreach (var field in members)
                    {
                        yield return GetExpressionForField(field);
                    }
                }

                Expression GetExpressionForField(ClassMember field)
                {
                    if (field.MemberType.IsInterface)
                    {
                        if (!field.Member.IsDefined(typeof(ConcreteTypeAttribute)))
                            throw new Exception("Fields of interface types must be decorated with ConcreteTypeAttribute");
                        else if (!field.MemberType.IsAssignableFrom(field.GetConcreteType()))
                            throw new Exception($"The specified concrete type for field '{field.Name}' doesn't implement the field type ({field.MemberType.Name})");
                    }

                    var format = Formats.Get(field.MemberType);

                    if (format == null)
                    {
                        if (Options.SerializeUnknownTypes)
                            return Block(GetBlock(PropertyOrField(convertedObj, field.Name), bufferExpr, field.MemberType));
                        else
                            throw new Exception($"Format not found for '{objType.Name}.{field.Name}'");
                    }
                    else
                    {
                        return Assign(PropertyOrField(convertedObj, field.Name), format.Deserialize(
                            new FormatContext(Formats, field.MemberType, typeof(TBuffer), bufferExpr, field.GetConcreteType())));
                    }
                }
            }
        }
    }
}
