using System.Net;
using System.Text;
using SuperSocket.Client;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

int count = 0;
var remo = new IPEndPoint(IPAddress.Parse("192.168.1.149"), 9090);

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
    // var response = await client.GetAsync("/todos/");
    // var content = await response.Content.ReadAsStringAsync();
    // Console.WriteLine(content);

    IConnector connector = new SocketConnector();
    
    try
    {
        var state = await connector.ConnectAsync(remo);
        if (!state.Result)
        {
            Console.WriteLine("Failed to connect to server.");
            return -1;
        }
        
        var connection = state.CreateConnection(new ConnectionOptions());
        Console.WriteLine($"Connected to {remo}-{count}");
        count++;
      
        //await connection.SendAsync("Hello\r\n"u8.ToArray());
        
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