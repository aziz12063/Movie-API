using System.Threading.Tasks;
using System;

namespace ApiApplication.Cache
{
    public interface IResponseCacheService
    {
        Task<string> GetCachedResponseAsync(string cacheKey);
        Task CacheResponseAsync(string cacheKey, string response, TimeSpan? absoluteExpiration = null);

    }
}
