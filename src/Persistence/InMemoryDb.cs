using codecrafters_redis.src.Models;

namespace codecrafters_redis.src.Persistence;

public class InMemoryDb
{
    private static readonly Dictionary<string, CacheEntry> _store = new Dictionary<string, CacheEntry>();
    private static InMemoryDb _instancia;

    private InMemoryDb() { }

    public static InMemoryDb Database
    {
        get { return _instancia ?? (_instancia = new InMemoryDb()); }
    }

    public Task SetAsync(string key, string value, long milliseconds = 0)
    {
        _store[key] = new CacheEntry(value, milliseconds);
        return Task.CompletedTask;
    }

    public Task<string> GetAsync(string key)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (entry.ExpiryTimestamp == 0 || entry.ExpiryTimestamp > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            {
                return Task.FromResult(entry.Value);
            }
            else
            {
                _store.Remove(key);
            }
        }

        return Task.FromResult<string>("");
    }
}
