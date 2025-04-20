using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class MessageCacheService
{
    private readonly IDistributedCache _cache;

    public MessageCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task SaveMessageAsync(string key, string message)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        await _cache.SetStringAsync(key, message, options);
    }

    public async Task<string?> GetMessageAsync(string key)
    {
        return await _cache.GetStringAsync(key);
    }

    public async Task DeleteMessageAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}
