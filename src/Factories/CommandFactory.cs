using codecrafters_redis.src.Commands;

namespace codecrafters_redis.src.Factories;

public static class CommandFactory
{
    public static ICommand CreateCommand(string[] args)
    {
        string commandName = args[0].ToLower();


        if (commandName == "ping")
        {
            return new PingCommand();
        }

        if (commandName == "get")
        {
            if (args.Length < 2)
                throw new ArgumentException("GET command requires a key argument");

            return new GetCommand(args[1]);
        }

        if (commandName == "set")
        {
            if (args.Length < 3)
                throw new ArgumentException("SET command requires a key and a value argument");

            if (args.Length == 5 && args[3].Equals("px", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!long.TryParse(args[4], out long milliseconds))
                {
                    throw new ArgumentException("SET command milliseconds argument must be an number");
                }

                return new SetCommand(args[1], args[2], milliseconds);
            }

            return new SetCommand(args[1], args[2]);
        }

        if (commandName == "echo")
        {
            if (args.Length < 2)
                throw new ArgumentException("ECHO command requires an argument");

            return new EchoCommand(args);
        }

        throw new ArgumentException("Invalid command");
    }
}
