using Makanak.Abstraction.IServices.Cashing;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Makanak.Services.Services.CashingImplement
{
    public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
    {
        public Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if(response == null) return Task.CompletedTask;
            
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializedResponse = JsonSerializer.Serialize(response, options);
            
            memoryCache.Set(cacheKey, serializedResponse, timeToLive);
            return Task.CompletedTask;
        }
        public Task<string?> GetCacheResponseAsync(string cacheKey)
        {
            var isCached = memoryCache.TryGetValue(cacheKey, out string? cachedResponse);

            return Task.FromResult(isCached ? cachedResponse : null);
        }

        public Task RemoveCacheResponseAsync(string cacheKey)
        {
            memoryCache.Remove(cacheKey);

            return Task.CompletedTask;
        }

    }
}
