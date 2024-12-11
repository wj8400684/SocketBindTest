using System.Net;
using System.Net.Sockets;
using System.Text;
using SuperSocket.Client;
using SuperSocket.Connection;

var remo = new IPEndPoint(IPAddress.Parse("192.168.124.53"), 8080);

var tasks = Enumerable
    .Range(0, 1)
    .Select(_ => RunConnectionAsync())
    .ToArray();

var rounds = await Task.WhenAll(tasks);

Console.WriteLine($"Rounds:{rounds.Length}");

Console.ReadKey();

async Task<int> RunConnectionAsync()
{
    IConnector connector = new SocketConnector();

    var state = await connector.ConnectAsync(remo);

    var connection = state.CreateConnection(new ConnectionOptions());

    for (int i = 0; i < 10; i++)
    {
        await connection.SendAsync(Encoding.UTF8.GetBytes("Hello World"));

        await Task.Delay(1000);
    }
    
    return 0;
}