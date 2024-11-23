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
        await client.ReceiveAsync(buffer);
        await client.SendAsync(Encoding.ASCII.GetBytes("+PONG\r\n"));
    }
}

