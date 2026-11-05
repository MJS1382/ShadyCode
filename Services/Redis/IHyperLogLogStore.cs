namespace PersonalBlog.Services.Redis;

public interface IHyperLogLogStore
{
    Task<long> GetUniqueItemsCountAsync(string key);
    Task<bool> ItemExistsAsync(string key); 
    Task AddAsync(string key, string value);
    Task RemoveAsync(string key);
}