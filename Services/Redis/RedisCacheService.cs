using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace PersonalBlog.Services.Redis;

public sealed class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _distributedCache.GetStringAsync(key);

        if (data is null)
            return default;

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var deserializedValue = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, deserializedValue,
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
        });
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}