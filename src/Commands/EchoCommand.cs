using codecrafters_redis.src.Models;

namespace codecrafters_redis.src.Commands;

public class EchoCommand : ICommand
{
    public EchoCommand(string[] messages)
    {
        Messages = messages.Skip(1).ToArray();
    }

    public string[] Messages { get; private set; }

    public Task<BulkString> ExecuteAsync()
    {
        return Task.FromResult(new BulkString(Messages));
    }
}


