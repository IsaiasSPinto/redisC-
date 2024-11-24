using codecrafters_redis.src.Models;
using codecrafters_redis.src.Persistence;

namespace codecrafters_redis.src.Commands;

public class GetCommand : ICommand
{
    public GetCommand(string key)
    {
        Key = key;
    }

    public string Key { get; private set; }

    public async Task<BulkString> ExecuteAsync()
    {
        var value = await InMemoryDb.Database.GetAsync(Key);

        return new BulkString(value);
    }
}

