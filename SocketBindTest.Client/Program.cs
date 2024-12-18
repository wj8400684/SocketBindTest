using System.Net;
using System.Text;
using SuperSocket.Client;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

//http://159.75.132.21:8080/api/connection/count

int count = 0;
var remo = new IPEndPoint(IPAddress.Parse("159.75.132.21"), 9000);

var list = new List<IConnection>();

var client = new HttpClient();
//client.BaseAddress = new Uri("http://192.168.1.149:9090");

for (int i = 0; i < 2000; i++)
{
    await RunConnectionAsync();
}

Console.WriteLine($"Rounds");

Console.ReadKey();

var tasks = Enumerable
    .Range(0, 2000)
    .Select(async _ => await RunConnectionAsync())
    .ToArray();

var rounds = await Task.WhenAll(tasks);

Console.WriteLine($"Rounds:{rounds.Length}");

Console.ReadKey();

async Task<int> RunConnectionAsync()
{
    //var response = await client.GetAsync("http://192.168.124.51:9090/");
    //var content = await response.Content.ReadAsStringAsync();
    //Console.WriteLine(content);

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

        list.Add(connection);
        //await connection.SendAsync("Hello\r\n"u8.ToArray());
        StartReceive(connection);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return -1;
    }


    return 0;
}

async void StartReceive(IConnection connection)
{
    await foreach (var pack in connection.RunAsync(new LinePipelineFilter()))
    {
        Console.WriteLine(pack.Text);
    }
}