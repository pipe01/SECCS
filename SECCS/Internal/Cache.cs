using System;
using System.Collections.Generic;

namespace SECCS.Internal
{
    internal class Cache<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> CacheDic = new Dictionary<TKey, TValue>();

        public TValue GetOrCreate(TKey key, Func<TValue> creator)
        {
            if (!CacheDic.TryGetValue(key, out var val))
            {
                return CacheDic[key] = creator();
            }

            return val;
        }
    }
}
