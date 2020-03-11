using System;
using System.Collections;
using System.Collections.Generic;

namespace SECCS
{
    public class FormatCollection<TFormat> : ICollection<TFormat> where TFormat : class, IFormat
    {
        private readonly List<TFormat> InnerList = new List<TFormat>();

        public int Count => this.InnerList.Count;

        public bool IsReadOnly => false;

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
