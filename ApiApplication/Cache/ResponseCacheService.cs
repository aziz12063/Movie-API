using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using System;

namespace ApiApplication.Cache
{
    public class ResponseCacheService: IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            return await _distributedCache.GetStringAsync(cacheKey);
        }

        public async Task CacheResponseAsync(string cacheKey, string response, TimeSpan? absoluteExpiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(absoluteExpiration ?? TimeSpan.FromMinutes(5)); // Default cache duration of 5 minutes

            await _distributedCache.SetStringAsync(cacheKey, response, options);
        }

    }
}
