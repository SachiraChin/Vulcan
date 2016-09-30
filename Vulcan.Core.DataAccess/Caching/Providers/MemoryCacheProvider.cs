using System;
using System.Runtime.Caching;

namespace Vulcan.Core.DataAccess.Caching.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        public void Set(string key, object value, TimeSpan expiration)
        {
            var policy = new CacheItemPolicy
            {
                Priority = CacheItemPriority.Default,
                SlidingExpiration = expiration
            };

            MemoryCache.Default.Set(key, value, policy);
        }

        public T Get<T>(string key)
        {
            if (MemoryCache.Default.Contains(key))
                return (T)MemoryCache.Default.Get(key);

            return default(T);
        }

        public void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        public void Dispose()
        {
            
        }
    }
}
