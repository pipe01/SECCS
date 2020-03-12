using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SECCS
{
    public class FormatCollection<TFormat> : ICollection<TFormat> where TFormat : class, IFormat
    {
        private readonly List<TFormat> InnerList = new List<TFormat>();

        public int Count => this.InnerList.Count;

        public bool IsReadOnly => false;

        internal void Discover()
        {
            var baseType = typeof(TFormat).GetGenericTypeDefinition();
            var bufferType = typeof(TFormat).GetGenericArguments()[0];

            foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (item.Namespace.StartsWith($"{nameof(SECCS)}.{nameof(Formats)}") && item.GetInterface(baseType.Name) != null)
                {
                    var specialized = item.MakeGenericType(bufferType);

                    Add((TFormat)Activator.CreateInstance(specialized));
                }
            }

            InnerList.Sort((a, b) => (a.GetType().GetCustomAttribute<FormatPriorityAttribute>()?.Priority ?? 0)
                                   - (b.GetType().GetCustomAttribute<FormatPriorityAttribute>()?.Priority ?? 0));
        }

        public TFormat? GetFor(Type type)
        {
            foreach (var item in InnerList)
            {
                if (item.CanFormat(type))
                    return item;
            }

            return null; //throw?
        }

        public void Add(TFormat item)
        {
            this.InnerList.Add(item);
        }

        public void AddRange(IEnumerable<TFormat> items)
        {
            this.InnerList.AddRange(items);
        }

        public void AddRange(params TFormat[] items)
        {
            this.InnerList.AddRange(items);
        }

        public void Clear()
        {
            this.InnerList.Clear();
        }

        public bool Contains(TFormat item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(TFormat[] array, int arrayIndex)
        {
            this.InnerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(TFormat item)
        {
            return this.InnerList.Remove(item);
        }

        public IEnumerator<TFormat> GetEnumerator()
        {
            return ((ICollection<TFormat>)this.InnerList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
