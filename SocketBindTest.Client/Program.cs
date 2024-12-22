using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using SocketBindTest.Client;
using SuperSocket.Client;
using SuperSocket.Client.Proxy;
using SuperSocket.Connection;
using SuperSocket.ProtoBase;

var commandPackageEncoder = new CommandPackageEncoder();
var joinPlay = new byte[]
{
    0x01, 0x1a, 0x2b, 0x3c, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf9,
    0x08, 0xc8, 0x01, 0x10, 0x01, 0x1a, 0xf1, 0x01,
    0x10, 0x01, 0x1a, 0x24, 0x39, 0x30, 0x36, 0x32,
    0x43, 0x35, 0x37, 0x32, 0x2d, 0x38, 0x36, 0x41,
    0x41, 0x2d, 0x42, 0x45, 0x34, 0x38, 0x2d, 0x39,
    0x42, 0x46, 0x44, 0x2d, 0x30, 0x31, 0x43, 0x43,
    0x42, 0x39, 0x38, 0x30, 0x36, 0x37, 0x39, 0x36,
    0x22, 0x0b, 0x37, 0x36, 0x5f, 0x79, 0x75, 0x37,
    0x43, 0x76, 0x55, 0x77, 0x45, 0x42, 0x0b, 0x7a,
    0x68, 0x5f, 0x43, 0x4e, 0x5f, 0x23, 0x48, 0x61,
    0x6e, 0x73, 0x4a, 0x0d, 0x31, 0x32, 0x2e, 0x31,
    0x31, 0x2e, 0x32, 0x30, 0x2e, 0x39, 0x30, 0x36,
    0x37, 0x5a, 0x02, 0x5f, 0x35, 0x60, 0x01, 0x6a,
    0x1b, 0x31, 0x5f, 0x69, 0x2f, 0x32, 0x30, 0x30,
    0x37, 0x38, 0x39, 0x39, 0x39, 0x39, 0x32, 0x33,
    0x32, 0x31, 0x37, 0x34, 0x33, 0x36, 0x30, 0x31,
    0x5f, 0x6e, 0x38, 0x36, 0x72, 0x40, 0x72, 0x56,
    0x70, 0x57, 0x55, 0x79, 0x65, 0x49, 0x6e, 0x6a,
    0x49, 0x49, 0x6e, 0x58, 0x46, 0x45, 0x58, 0x65,
    0x31, 0x4f, 0x74, 0x70, 0x32, 0x70, 0x6f, 0x56,
    0x79, 0x62, 0x67, 0x59, 0x31, 0x5a, 0x49, 0x77,
    0x71, 0x6d, 0x4c, 0x6f, 0x33, 0x64, 0x70, 0x51,
    0x71, 0x51, 0x49, 0x4a, 0x69, 0x52, 0x5a, 0x68,
    0x76, 0x66, 0x71, 0x36, 0x5a, 0x6f, 0x54, 0x71,
    0x57, 0x79, 0x34, 0x39, 0x37, 0x33, 0x78, 0x01,
    0x80, 0x01, 0x03, 0xa0, 0x01, 0xe0, 0x98, 0xb5,
    0xf1, 0x0c, 0xaa, 0x01, 0x08, 0x4b, 0x55, 0x41,
    0x49, 0x53, 0x48, 0x4f, 0x55, 0xb2, 0x01, 0x06,
    0x49, 0x50, 0x48, 0x4f, 0x4e, 0x45, 0xc2, 0x01,
    0x04, 0x31, 0x38, 0x2e, 0x32, 0xca, 0x01, 0x07,
    0x69, 0x6f, 0x73, 0x31, 0x38, 0x2e, 0x32, 0xd2,
    0x01, 0x03, 0x4d, 0x41, 0x43, 0xda, 0x01, 0x01,
    0x61
};

var body = joinPlay.AsSpan(16);

var commandPackage = CommandPackage.Parser.ParseFrom(body);

commandPackage.TryParse<EnterRoomPackage>(out var enterRoomPackage);

int count = 0;
var remo = new DnsEndPoint("www.baidu.com", 443,
    AddressFamily.InterNetwork); //new IPEndPoint(IPAddress.Parse("192.168.1.149"), 9090);

var list = new List<IConnection>();

var client = new HttpClient();
//client.BaseAddress = new Uri("http://192.168.1.149:9090");

for (int i = 0; i < 2000; i++)
{
    await RunConnectionLiveAsync();
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


async Task<int> RunConnectionLiveAsync()
{
    //var response = await client.GetAsync("http://192.168.124.51:9090/");
    //var content = await response.Content.ReadAsStringAsync();
    //Console.WriteLine(content);
    //new SocketConnector();

    int m = 0;
    var s1 = "127.0.0.1,880,wujun,wujun43435".AsSpan();

    var array = s1.Split(',');
    while (array.MoveNext())
    {
        var start = array.Current.Start.Value;
        var length = array.Current.End.Value - array.Current.Start.Value;

        var s2 = s1.Slice(start, length).ToString();
    }

    var connectors = new List<ConnectorBase>
    {
        new Socks5Connector(new IPEndPoint(IPAddress.Parse("159.75.132.21"), 38438)),
        new EnterRoomConnector()
    };
    
    
    


    IConnector connector = new Socks5Connector(new IPEndPoint(IPAddress.Parse("159.75.132.21"), 38438));

    try
    {
        var state = await connector.ConnectAsync(new IPEndPoint(IPAddress.Parse("103.107.218.170"), 443));
        if (!state.Result)
        {
            Console.WriteLine("Failed to connect to server.");
            return -1;
        }

        var connection = state.CreateConnection(new ConnectionOptions());
        var packageStream = connection.GetPackageStream(new CommandPackagePipeLineFilter());

        Console.WriteLine($"Connected to {remo}-{count}");
        count++;


        // var protocol = new CommandPackageProtocol(connection);
        //
        // var response = await protocol.SendEnterRoomAsync(new EnterRoomPackage
        // {
        //     Sys = "",
        // });
        //
        // if (response.TryParse<EnterRoomPackage>(out var enterRoomPackage1))
        // {
        //     
        // }
        //
        //
        // await foreach (var package in protocol.ReceiveAsync())
        // {
        // }

        list.Add(connection);

        // await connection.SendAsync(commandPackageEncoder,
        //     CommandPackage.New(CommandPackage.Types.CommandType.EnterRoom, enterRoomPackage!));
        // var result = await packageStream.ReceiveAsync();

        LiveStartReceive(connection, packageStream);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return -1;
    }

    return 0;
}

async void LiveStartReceive(IConnection connection, IAsyncEnumerator<CommandPackage> packageStream)
{
    try
    {
        while (true)
        {
            await connection.SendAsync(commandPackageEncoder,
                CommandPackage.New(CommandPackage.Types.CommandType.HeartBeat, new GeneralPackage
                {
                    TimeStamp = DateTime.UtcNow.ToTimeStamp13()
                }));
            var result = await packageStream.ReceiveAsync();
            if (result == null)
                break;

            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }
    finally
    {
        Console.WriteLine("断开连接了");
    }
}


async Task<int> RunConnectionAsync()
{
    //var response = await client.GetAsync("http://192.168.124.51:9090/");
    //var content = await response.Content.ReadAsStringAsync();
    //Console.WriteLine(content);
    //new SocketConnector();

    IConnector connector =
        new Socks5Connector(new IPEndPoint(IPAddress.Parse("103.107.218.170"), 443));

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
        await connection.SendAsync(joinPlay);

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