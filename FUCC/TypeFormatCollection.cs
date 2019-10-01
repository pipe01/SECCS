using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FUCC
{
    public class ReadOnlyTypeFormatCollection : IEnumerable<ITypeFormat>
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
        public ITypeFormat Get(Type type) => Formats.FirstOrDefault(o => o.CanFormat(type));
    }

    public class TypeFormatCollection : ReadOnlyTypeFormatCollection
    {
        /// <summary>
        /// Registers a type format of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type format type</typeparam>
        public void Register<T>() where T : ITypeFormat, new()
        {
            Formats.Add(new T());
        }

        internal void Register(IEnumerable<ITypeFormat> formats)
        {
            Formats.AddRange(formats);
        }
    }
}
