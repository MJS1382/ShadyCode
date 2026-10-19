using StackExchange.Redis;

namespace PersonalBlog.Services.Redis;

public sealed class HyperLogLogStore : IHyperLogLogStore
{
    private readonly IDatabase _database;

    public HyperLogLogStore(IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
    }

    public async Task<bool> ItemExists(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    public async Task<long> GetUniqueItemsCountAsync(string key)
    {
        return await _database.HyperLogLogLengthAsync(key);
    }

    public async Task AddAsync(string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Value cannot be null or empty.");
        }

        await _database.HyperLogLogAddAsync(key, value.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}