using FUCC.DefaultFormats;
using FUCC.Exceptions;
using FUCC.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace FUCC
{
    using static Expression;

    public class FuccFormatter<TBuffer>
    {
        private const byte Magic = 243;
        private const byte MagicWithSignature = 244;

        private static readonly ITypeFormat[] DefaultFormats;

        private readonly IDictionary<Type, Func<TBuffer, object>> Deserializers = new Dictionary<Type, Func<TBuffer, object>>();
        private readonly IDictionary<Type, Action<TBuffer, object>> Serializers = new Dictionary<Type, Action<TBuffer, object>>();

        private readonly List<ITypeFormat> Formats;
        private readonly FuccOptions Options;

        static FuccFormatter()
        {
            DefaultFormats = typeof(ITypeFormat).Assembly
                .GetTypes()
                .Where(o => typeof(ITypeFormat).IsAssignableFrom(o) && o.Namespace == typeof(ValueTupleFormat).Namespace)
                .OrderByDescending(o => o.GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0)
                .Select(o => (ITypeFormat)Activator.CreateInstance(o))
                .ToArray();
        }

        public FuccFormatter(FuccOptions options = null) : this(Enumerable.Empty<ITypeFormat>(), options)
        {
        }
        public FuccFormatter(IEnumerable<ITypeFormat> formats, FuccOptions options = null)
        {
            this.Formats = new List<ITypeFormat>();
            this.Options = options ?? new FuccOptions();

            Formats.AddRange(DefaultFormats);
            Formats.AddRange(formats);
        }

        public void AddFormat<T>() where T : ITypeFormat, new()
            => Formats.Add(new T());

        private static IEnumerable<FieldInfo> GetFields(Type t)
            => t.GetFields(BindingFlags.Public | BindingFlags.Instance).Where(o => !o.IsDefined(typeof(IgnoreDataMemberAttribute), false)).ToArray();

        private ITypeFormat GetFormat(Type t)
            => Formats.Find(o => o.CanFormat(t));

        public void Serialize<T>(TBuffer buffer, T obj)
            => Serialize(buffer, obj, typeof(T));
        
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
                    exprs.Add(GetFormat(typeof(byte)).Serialize(new FormatContextWithValue(Formats, typeof(byte), bufferParam, Constant((byte)(Options.WriteStructureSignature ? MagicWithSignature : Magic)))));

                    if (Options.WriteStructureSignature)
                    {
                        var hash = ClassSignature.Get(type);
                        exprs.Add(GetFormat(typeof(string)).Serialize(new FormatContextWithValue(Formats, typeof(string), bufferParam, Constant(hash))));
                    }
                }

                exprs.AddRange(GetBlock(objParam, bufferParam, type));

                Serializers[type] = ser = Lambda<Action<TBuffer, object>>(Block(exprs), bufferParam, objParam).Compile();
            }

            ser(buffer, obj);

            IEnumerable<Expression> GetBlock(Expression objExpr, Expression bufferExpr, Type objType)
            {
                var convertedObj = Convert(objExpr, objType);

                foreach (var field in GetFields(objType))
                {
                    var format = GetFormat(field.FieldType);

                    if (format == null)
                    {
                        if (Options.SerializeUnknownTypes)
                            yield return Block(GetBlock(Field(convertedObj, field), bufferExpr, field.FieldType));
                        else
                            throw new Exception($"Format not found for '{type.Name}.{field.Name}'");
                    }
                    else
                    {
                        yield return format.Serialize(new FormatContextWithValue(Formats, field.FieldType, bufferExpr, Field(convertedObj, field)));
                    }
                }
            }
        }

        public T Deserialize<T>(TBuffer buffer)
            => (T)Deserialize(buffer, typeof(T));

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

                    if (Options.CheckStructureSignature)
                    {
                        var hash = ClassSignature.Get(type);
                        /*
                         byte magic = Read<byte>();
                         if (magic == 244)
                         {
                             if (Read<string>() != "HASH")
                             {
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
                                IfThen(NotEqual(Read<string>(), Constant(hash)), InvalidHeaderException.Throw("Class structure signature mismatch")),
                                IfThen(NotEqual(magicVar, Constant(Magic)), InvalidHeaderException.Throw("Invalid magic number")))));
                    }
                    else
                    {
                        exprs.Add(Block(new[] { magicVar },
                            Assign(magicVar, Read<byte>()),
                            IfThen(
                                NotEqual(magicVar, Constant(Magic)),
                                InvalidHeaderException.Throw("Invalid magic number"))));
                    }
                }

                exprs.AddRange(GetBlock(objVar, bufferParam, type));
                exprs.Add(objVar);

                Deserializers[type] = des = Lambda<Func<TBuffer, object>>(Block(new[] { objVar }, exprs), bufferParam).Compile();

                Expression Read<T>() => GetFormat(typeof(T)).Deserialize(new FormatContext(Formats, typeof(T), bufferParam));
            }

            return des(buffer);

            IEnumerable<Expression> GetBlock(Expression objExpr, Expression bufferExpr, Type objType)
            {
                var convertedObj = Convert(objExpr, objType);

                yield return Assign(objExpr, New(objType));

                foreach (var field in GetFields(objType))
                {
                    var format = GetFormat(field.FieldType);

                    if (format == null)
                    {
                        if (Options.SerializeUnknownTypes)
                            yield return Block(GetBlock(Field(convertedObj, field), bufferExpr, field.FieldType));
                        else
                            throw new Exception($"Format not found for '{objType.Name}.{field.Name}'");
                    }
                    else
                    {
                        yield return Assign(Field(convertedObj, field), format.Deserialize(new FormatContext(Formats, field.FieldType, bufferExpr)));
                    }
                }
            }
        }
    }
}
