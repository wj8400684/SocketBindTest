using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;

namespace SocketBindTest.Server;

internal sealed class TelnetConnectionHandler : ConnectionHandler
{
    private readonly List<IPAddress> _ipAddresses;
    private readonly ILogger<TelnetConnectionHandler> _logger;
    private readonly DnsEndPoint _remoteEndPoint = new("www.bejson.com", 443, AddressFamily.InterNetwork);

    public TelnetConnectionHandler(ILogger<TelnetConnectionHandler> logger)
    {
        _logger = logger;
        _ipAddresses = Dns.GetHostAddresses(Dns.GetHostName())
            .Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToList();

        foreach (var address in _ipAddresses)
        {
            _logger.LogInformation($"本地ip{address}");
        }
    }

    private IPAddress? GetLocalIpAddress(IPAddress address)
    {
        return _ipAddresses.FirstOrDefault(s => s.Equals(address));
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var port = connection.RemoteEndPoint switch
        {
            IPEndPoint ip => ip.Port,
            DnsEndPoint dns => dns.Port,
            _ => throw new NotImplementedException(),
        };

        if (connection.LocalEndPoint is not IPEndPoint localEndPoint)
        {
            _logger.LogWarning("ip不正确");
            return;
        }

        var address = localEndPoint.Address.MapToIPv4();
        
        _logger.LogInformation("新连接，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.ExclusiveAddressUse = false;
            socket.Bind(new IPEndPoint(address, port));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "绑定ip失败，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
            return;
        }

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        using var cancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed, timeout.Token);

        try
        {
            await socket.ConnectAsync(_remoteEndPoint, cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接远程服务器失败，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
            return;
        }

        Stream stream = new NetworkStream(socket, true);

        _logger.LogInformation("开始传输数据-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

        try
        {
            await connection.Transport.Input.CopyToAsync(stream, CancellationToken.None);
            await stream.CopyToAsync(connection.Transport.Output, CancellationToken.None);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ip不正确");
        }
        finally
        {
            _logger.LogWarning("断开连接啦，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
        }
    }

    private bool OnRemoteCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain,
        SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private X509Certificate OnLocalCertificateSelectionCallback(object sender, string targetHost,
        X509CertificateCollection localCertificates, X509Certificate? remoteCertificate, string[] acceptableIssuers)
    {
        return null;
    }
}