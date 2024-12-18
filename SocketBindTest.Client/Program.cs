using System.Net;
using System.Net.Sockets;
using SuperSocket.Client;
using SuperSocket.Client.Proxy;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

int count = 0;
var remo = new DnsEndPoint("www.baidu.com",443,AddressFamily.InterNetwork);//new IPEndPoint(IPAddress.Parse("192.168.1.149"), 9090);

var list = new List<IConnection>();

var client = new HttpClient();
//client.BaseAddress = new Uri("http://192.168.1.149:9090");

for (int i = 0; i < 1000; i++)
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
    //new SocketConnector();

    IConnector connector =
        new Socks5Connector(new IPEndPoint(IPAddress.Parse("192.168.1.149"), 38438), "wujun", "wujun");

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