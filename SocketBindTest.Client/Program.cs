using System.Net;
using System.Net.Sockets;
using System.Text;
using SuperSocket.Client;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

int count = 0;
var remo = new IPEndPoint(IPAddress.Parse("192.168.1.149"), 8888);

var client = new HttpClient();
client.BaseAddress = new Uri("http://192.168.1.149:9090");

var tasks = Enumerable
    .Range(0, 10000)
    .Select(_ => RunConnectionAsync())
    .ToArray();

var rounds = await Task.WhenAll(tasks);

Console.WriteLine($"Rounds:{rounds.Length}");

Console.ReadKey();



async Task<int> RunConnectionAsync()
{
    //var response = await client.GetAsync("/todos/");
    //var content = await response.Content.ReadAsStringAsync();
    //Console.WriteLine(content);

    IConnector connector = new SocketConnector();

    try
    {
        var state = await connector.ConnectAsync(remo);

        var connection = state.CreateConnection(new ConnectionOptions());
        Console.WriteLine($"Connected to {remo}-{count}");
        count++;
        // for (int i = 0; i < 10; i++)
        // {
        //     await connection.SendAsync(Encoding.UTF8.GetBytes("Hello World"));
        //
        //     await Task.Delay(1000);
        // }

        await foreach (var pack in connection.RunAsync(new LinePipelineFilter()))
        {
            Console.WriteLine(pack.Text);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return -1;
    }

    

    return 0;
}