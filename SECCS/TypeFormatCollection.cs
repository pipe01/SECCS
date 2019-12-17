using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SECCS
{
    public interface IReadOnlyTypeFormatCollection : IEnumerable<ITypeFormat>
    {
        ITypeFormat Get<T>();
        ITypeFormat Get(Type type);
    }

    public class ReadOnlyTypeFormatCollection<TBuffer> : IReadOnlyTypeFormatCollection
    {
        protected readonly List<ITypeFormat> Formats = new List<ITypeFormat>();

        public IEnumerator<ITypeFormat> GetEnumerator() => this.Formats.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.Formats.GetEnumerator();


        /// <summary>
        /// Gets the first type format for <typeparamref name="T"/> from the list of registered formats.
        /// </summary>
        public ITypeFormat Get<T>() => Get(typeof(T));

        /// <summary>
        /// Gets the first type format for <paramref name="type"/> from the list of registered formats.
        /// </summary>
        public ITypeFormat Get(Type type) => Formats.Find(o => o.CanFormat(type));
    }

    public class TypeFormatCollection<TBuffer> : ReadOnlyTypeFormatCollection<TBuffer>
    {
        internal TypeFormatCollection(IEnumerable<ITypeFormat> formats)
        {
            Formats.AddRange(formats);
        }

        public TypeFormatCollection()
        {
        }

        /// <summary>
        /// Registers a type format of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type format type</typeparam>
        public TypeFormatCollection<TBuffer> Register<T>() where T : ITypeFormat, new()
        {
            Formats.Add(new T());

            return this;
        }

        public TypeFormatCollection<TBuffer> Register<T>(Action<TBuffer, T> writer, Func<TBuffer, T> reader, bool prepend = false)
        {
            var format = new LambdaFormat<TBuffer, T>(reader, writer);

            if (prepend)
                Formats.Insert(0, format);
            else
                Formats.Add(format);

            return this;
        }

        public void RemoveAll(Predicate<ITypeFormat> predicate) => Formats.RemoveAll(predicate);

        public void SortByPriority() => Formats.Sort((a, b) => 
                (b.GetType().GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0).CompareTo(
                    a.GetType().GetCustomAttribute<PriorityAttribute>()?.Priority ?? 0));

        internal void Register(IEnumerable<ITypeFormat> formats)
        {
            Formats.AddRange(formats);
        }
    }
}
