using System;

namespace DeloitteMvcApiSample.Helpers
{
    public interface IAbstractCacheReporitory<T> where T:class, new()
    {
        T Get(string cacheItemName);
        
        /// <summary>
        /// 
        /// </summary>
        T GetOrSet(string cacheItemName, Func<T> valueFunction);
        T GetOrSet(string cacheItemName, int cacheMins, Func<T> valueDelegate);
        

        void Set(string cacheItemName, object cachedObject);
        void Set(string cacheItemName, object cachedObject, int cacheMins);

        /// <summary>
        /// 
        /// </summary>
        void SetIfNotExists(string cacheItemName, Func<T> valueDelegate);
    }
}