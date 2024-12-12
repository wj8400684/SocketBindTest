using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;

namespace SocketBindTest.Server;

internal sealed class TelnetConnectionHandler : ConnectionHandler
{
    private readonly List<IPAddress> _ipAddresses;
    private readonly ILogger<TelnetConnectionHandler> _logger;
    private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new();
    private readonly IPEndPoint _remoteEndPoint;

    public TelnetConnectionHandler(IOptions<ForwardOptions> options, ILogger<TelnetConnectionHandler> logger)
    {
        _logger = logger;
        var forwardOptions = options.Value;
        _remoteEndPoint = new IPEndPoint(forwardOptions.ForwardIpAddress!, forwardOptions.ForwardPort);
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

        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.ExclusiveAddressUse = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
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

        Stream stream = new NetworkStream(socket, false);

        // if (_remoteEndPoint.Port == 443) 
        // { 
        //     var ssl = new SslStream(stream, false);

        //     try
        //     {
        //         await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
        //         {
        //             TargetHost = _remoteEndPoint.Host,
        //             RemoteCertificateValidationCallback = OnRemoteCertificateValidationCallback,
        //             LocalCertificateSelectionCallback = OnLocalCertificateSelectionCallback,
        //         }, cancellationTokenSource.Token);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex,"ssl验证失败-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
        //connection.LocalEndPoint);
        //         return;
        //     }

        //     stream = ssl;
        // }

        _logger.LogInformation("开始传输数据-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

        _connections.TryAdd(connection.ConnectionId, connection);

        try
        {
            var task = connection.Transport.Input.CopyToAsync(stream, CancellationToken.None);
            var task1 = stream.CopyToAsync(connection.Transport.Output, CancellationToken.None);
            await Task.WhenAny(task, task1);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "ip不正确");
        }
        finally
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                //
            }

            _connections.TryRemove(connection.ConnectionId, out _);
            _logger.LogWarning("断开连接啦，远程地址-{RemoteEndPoint}-{LocalEndPoint}-连接数-{Count}", connection.RemoteEndPoint,
                connection.LocalEndPoint, _connections.Count);
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