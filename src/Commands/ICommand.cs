using codecrafters_redis.src.Models;

namespace codecrafters_redis.src.Commands;

public interface ICommand
{
    Task<BulkString> ExecuteAsync();
}
