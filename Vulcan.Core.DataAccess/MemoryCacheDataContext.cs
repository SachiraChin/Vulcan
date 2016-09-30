using System;
using Vulcan.Core.DataAccess.Caching;
using Vulcan.Core.DataAccess.Caching.Providers;

namespace Vulcan.Core.DataAccess
{
    public class MemoryCacheDataContext : IDisposable
    {
        public string TenantId => _tenantId;

        private readonly string _tenantId;

        private readonly ICacheProvider _cacheProvider;

        public MemoryCacheDataContext(string tenantId)
        {
            _tenantId = tenantId ?? "shared";
            _cacheProvider = new MemoryCacheProvider();
        }


        public void Set(string key, object value, TimeSpan expiration)
        {
            _cacheProvider.Set($"{_tenantId}_{key}", value, expiration);
        }

        public T Get<T>(string key)
        {
            return _cacheProvider.Get<T>($"{_tenantId}_{key}");
        }

        public void Remove(string key)
        {
            _cacheProvider.Remove($"{_tenantId}_{key}");
        }

        public void Dispose()
        {
            _cacheProvider.Dispose();
        }
    }
}
