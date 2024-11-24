
using codecrafters_redis.src.Models;
using codecrafters_redis.src.Persistence;
using System.Text;

namespace codecrafters_redis.src.Commands;

public class SetCommand : ICommand
{
    public SetCommand(string key, string value, long milliseconds = 0)
    {
        Key = key;
        Value = value;
        Milliseconds = milliseconds;
    }

    public string Key { get; private set; }
    public string Value { get; private set; }
    public long Milliseconds { get; private set; }

    public async Task<BulkString> ExecuteAsync()
    {
        await InMemoryDb.Database.SetAsync(Key, Value, Milliseconds);

        return new BulkString("+OK");
    }
}
