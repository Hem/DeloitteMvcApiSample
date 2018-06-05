using System;
using System.Runtime.Caching;

namespace DeloitteMvcApiSample.Helpers
{
    public abstract class AbstractCacheReporitory<T> : IAbstractCacheReporitory<T> where T:class, new() 
    {
        protected readonly MemoryCache Cache = new MemoryCache(typeof(T).Name);

        public abstract int DefaultCacheMins { get; set; }
     


        public T GetOrSet(string cacheItemName, Func<T> valueDelegate)
        {
            return GetOrSet(cacheItemName, DefaultCacheMins, valueDelegate);
        }

        public T GetOrSet(string cacheItemName, int cacheMins, Func<T> valueDelegate)
        {
            var value = Get(cacheItemName);

            if (value == null)
            {
                value = valueDelegate();
                Set(cacheItemName, value, cacheMins);
            }

            return value;
        }


        public T Get(string cacheItemName)
        {
            var cachedObject = (T)Cache[cacheItemName];

            return cachedObject;
        }

        public void Set(string cacheItemName, object cachedObject)
        {
            Set(cacheItemName, cachedObject, DefaultCacheMins);
        }

        public void Set(string cacheItemName, object cachedObject, int cacheMins)
        {
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheMins)
            };
            
            Cache.Set(cacheItemName, cachedObject, policy);
        }

        public void SetIfNotExists(string cacheItemName, Func<T> valueDelegate)
        {
            if(!Cache.Contains(cacheItemName))
            {
                var value = valueDelegate();
                Set(cacheItemName, value, DefaultCacheMins);
            }
        }
    }
}