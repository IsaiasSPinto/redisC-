
using codecrafters_redis.src.Models;

namespace codecrafters_redis.src.Commands;

public class PingCommand : ICommand
{
    public Task<BulkString> ExecuteAsync()
    {
        return Task.FromResult(new BulkString("+PONG"));
    }
}
