namespace codecrafters_redis.src.Models;

public class CacheEntry
{
    public CacheEntry(string value, long milliseconds)
    {
        Value = value;
        if (milliseconds > 0)
        {
            ExpiryTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + milliseconds;
        }
    }

    public string Value { get; set; }
    public long ExpiryTimestamp { get; set; }
}
