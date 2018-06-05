using System;
using System.Runtime.Caching;
using System.Security.Claims;

namespace DeloitteMvcApiSample.Helpers
{

    public static class CacheKeyGenerator
    {
        public static string GetUserCacheToken(this ClaimsPrincipal principal)
        {
            var name = principal.Identity.Name;
            var issuedAt = principal.FindFirst("iat")?.Value;
            return $"{name}-{issuedAt}";
        }
    }


    public static class CacheHelper
    {

        public static bool Exists<T>(string cacheItemName)
        {
            MemoryCache cache = MemoryCache.Default;
            
            return cache.Contains(cacheItemName);
        }

        public static T GetOrSet<T>(string cacheItemName, int cacheMins, Func<T> objectGetterFunction)
        {
            var value = Get<T>(cacheItemName);

            if(value == null)
            {
                value = objectGetterFunction();
                Set<T>(cacheItemName, value, cacheMins);
            }

            return value;
        }

        public static T Get<T>(string cacheItemName)
        {
            MemoryCache cache = MemoryCache.Default;

            var cachedObject = (T)cache[cacheItemName];

            return cachedObject;
        }

        public static void Set<T>(string cacheItemName, object cachedObject, int cacheMins)
        {
            MemoryCache cache = MemoryCache.Default;

            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheMins)
            };

            cache.Set(cacheItemName, cachedObject, policy);
        }



    }
}