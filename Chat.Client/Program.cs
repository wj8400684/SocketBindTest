using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

var count = 0;
var remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.149"), 8080);

for (var i = 0; i < 2000; i++)
{
    await RunConnectionAsync();
}

Console.WriteLine($"Rounds");
Console.ReadKey();

async Task<int> RunConnectionAsync()
{
    try
    {
        await Task.Delay(Random.Shared.Next(100));
        
        var connection = new HubConnectionBuilder()
            .WithUrl($"ws://{remoteEndPoint.Address}:{remoteEndPoint.Port}/api/chat")
            .AddJsonProtocol()
            .WithAutomaticReconnect()
            .Build();
    
        await connection.StartAsync();

        count++;
        
        Console.WriteLine($"Connected {count}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return -1;
    }

    return 0;
}
