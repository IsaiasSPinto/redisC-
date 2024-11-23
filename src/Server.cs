using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Logs from your program will appear here!");

Dictionary<string, CacheEntry> _data = new Dictionary<string, CacheEntry>();

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();


while (true)
{
    var client = await server.AcceptSocketAsync();
    _ = HandleClientAsync(client);
}


async Task HandleClientAsync(Socket client)
{
    while (client.Connected)
    {
        var buffer = new byte[1024];
        var bytes = await client.ReceiveAsync(buffer);

        string input = Encoding.ASCII.GetString(buffer, 0, bytes);

        var args = BulkStringToStringArray(input.Replace("\\r", "\r").Replace("\\n", "\n"));


        if (String.Equals(args[0], "PING", StringComparison.InvariantCultureIgnoreCase))
        {
            await client.SendAsync(Encoding.ASCII.GetBytes("+PONG\r\n"));
        }

        if (String.Equals(args[0], "ECHO", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("ECHO command requires an argument");
            }

            var response = Encoding.ASCII.GetBytes(ConvertToBulkString(args[1]));
            await client.SendAsync(response);
        }

        if (String.Equals(args[0], "SET", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("SET command requires a key and a value");
            }

            long? expireTimestamp = null;

            if (args.Length == 5 && String.Equals(args[3], "PX", StringComparison.InvariantCultureIgnoreCase))
            {
                if (long.TryParse(args[4], System.Globalization.CultureInfo.InvariantCulture, out long ttl))
                {
                    expireTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + ttl;
                }
                else
                {
                    throw new ArgumentException("Invalid TTL value");
                }
            }

            _data[args[1]] = new CacheEntry
            {
                Value = args[2],
                ExpiryTimestamp = expireTimestamp
            };


            await client.SendAsync(Encoding.ASCII.GetBytes("+OK\r\n"));
        }

        if (String.Equals(args[0], "GET", StringComparison.InvariantCultureIgnoreCase))
        {
            if (args.Length < 2)
            {
                await client.SendAsync(Encoding.ASCII.GetBytes("$-1\r\n"));
            }

            if (_data.TryGetValue(args[1], out var cacheEntry))
            {
                if (cacheEntry.ExpiryTimestamp.HasValue && cacheEntry.ExpiryTimestamp.Value < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                {
                    _data.Remove(args[1]);
                    await client.SendAsync(Encoding.ASCII.GetBytes("$-1\r\n"));
                }
                else
                {
                    var response = Encoding.ASCII.GetBytes(ConvertToBulkString(cacheEntry.Value));
                    await client.SendAsync(response);
                }
            }
            else
            {
                await client.SendAsync(Encoding.ASCII.GetBytes("$-1\r\n"));
            }
        }


    }
}



static string ConvertToBulkString(string data)
{
    return $"${data.Length}\r\n{data}\r\n";
}

static string[] BulkStringToStringArray(string data)
{
    if (data[0] == '*')
    {
        var args = data
            .Substring(2).TrimStart().Split("$").Skip(1)
            .Select(x =>
            {

                if (int.TryParse(x[1].ToString(), System.Globalization.CultureInfo.InvariantCulture, out _))
                    return x.Substring(2).Trim();


                return x.Substring(1).Trim();
            });

        return args.ToArray();
    }

    return [data.Substring(1).Trim()];
}

public class CacheEntry
{
    public string Value { get; set; }
    public long? ExpiryTimestamp { get; set; }
}

