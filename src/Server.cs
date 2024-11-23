using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new TcpListener(IPAddress.Any, 6379);
server.Start();


while (true)
{
    var client = await server.AcceptSocketAsync();
    _ = HandleClientAsync(client);
}


async static Task HandleClientAsync(Socket client)
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
            .Select(x => x.Substring(1).Trim());

        return args.ToArray();
    }

    return [data.Substring(1).Trim()];
}

