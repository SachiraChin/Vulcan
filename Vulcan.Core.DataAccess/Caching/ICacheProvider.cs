using System;

namespace Vulcan.Core.DataAccess.Caching
{
    public interface ICacheProvider : IDisposable
    {
        void Set(string key, object value, TimeSpan expiration);
        T Get<T>(string key);
        void Remove(string key);
    }
}
