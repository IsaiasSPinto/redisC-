using codecrafters_redis.src.Factories;
using codecrafters_redis.src.Models;
using System.ComponentModel.DataAnnotations;
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


async Task HandleClientAsync(Socket client)
{


    while (client.Connected)
    {
        var buffer = new byte[1024];
        var bytes = await client.ReceiveAsync(buffer);

        string input = Encoding.ASCII.GetString(buffer, 0, bytes);

        if (string.IsNullOrEmpty(input.Trim()))
        {
            continue;
        }

        var args = BulkString.ExtractBulkString(input);


        try
        {
            var command = CommandFactory.CreateCommand(args);

            var result = await command.ExecuteAsync();

            await client.SendAsync(Encoding.ASCII.GetBytes(result.Value), SocketFlags.None);
        }
        catch (Exception ex)
        {
            await client.SendAsync(Encoding.ASCII.GetBytes($"An unexpected error as occured: {ex.Message}\r\n"), SocketFlags.None);
        }
    }
}




