using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Abstraction.IServices.Cashing
{
    public interface ICacheService
    {
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string?> GetCacheResponseAsync(string cacheKey);
        Task RemoveCacheResponseAsync(string cacheKey);
    }
}