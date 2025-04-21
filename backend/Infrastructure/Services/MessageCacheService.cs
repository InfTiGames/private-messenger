using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;

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

    public async Task<Dictionary<string, string>> ExportAllMessagesAsync()
    {
        var keys = new List<string>(); // Получите список ключей из Redis (зависит от реализации)
        var messages = new Dictionary<string, string>();

        foreach (var key in keys)
        {
            var message = await _cache.GetStringAsync(key);
            if (message != null)
            {
                messages[key] = message;
            }
        }

        return messages;
    }

    public async Task DeleteAllMessagesForUserAsync(string userId)
    {
        // Implement the logic to delete all messages for the user from the cache
        // Example:
        Console.WriteLine($"Deleting all messages for user with ID: {userId}");
        await Task.CompletedTask;
    }
}

[TestFixture]
public class MessageCacheServiceTests
{
    [Test]
    public async Task SaveMessageAsync_ShouldStoreMessageInCache()
    {
        var cacheMock = new Mock<IDistributedCache>();
        var service = new MessageCacheService(cacheMock.Object);
        var key = "testKey";
        var message = "testMessage";

        await service.SaveMessageAsync(key, message);

        cacheMock.Verify(
            c => c.SetStringAsync(key, message, It.IsAny<DistributedCacheEntryOptions>(), default),
            Times.Once
        );
    }
}
