using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Connections;
namespace Kestrel.Socket.Server.ConnectionHandlers;

/// <summary>
/// 转发客户端
/// </summary>
/// <param name="logger"></param>
internal sealed class ForwardConnectionHandler(
    ILogger<ForwardConnectionHandler> logger)
    : ConnectionHandler
{
    private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new();
    private readonly DnsEndPoint _target = new("192.168.124.51", 27727, AddressFamily.InterNetwork);
    
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
            logger.LogWarning("ip不正确");
            return;
        }

        logger.LogInformation("新连接，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

        using var socket = new System.Net.Sockets.Socket(_target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

#if !OSX
        var address = localEndPoint.Address.MapToIPv4();

        try
        {
            socket.ExclusiveAddressUse = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(address, port));
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
        {
            logger.LogError(ex, "绑定ip失败，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
            return;
        }
        catch (Exception e)
        {
            logger.LogError(e, "设置socket参数失败未知错误-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
            return;
        }

#endif

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        using var cancellationTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed, timeout.Token);

        try
        {
            await socket.ConnectAsync(_target, cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "连接远程服务器失败，远程地址-{forwardEndPoint}", _target);
            return;
        }

        var stream = new NetworkStream(socket, true);

        logger.LogInformation("开始传输数据-{RemoteEndPoint}-{LocalEndPoint}-{Count}", connection.RemoteEndPoint,
            connection.LocalEndPoint, _connections.Count);

        _connections.TryAdd(connection.ConnectionId, connection);

        try
        {
            var task = connection.Transport.Input.CopyToAsync(stream, CancellationToken.None);
            var task1 = stream.CopyToAsync(connection.Transport.Output, CancellationToken.None);
            await Task.WhenAny(task, task1);
        }
        catch (Exception e)
        {
            logger.LogError(e, "ip不正确");
        }

        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
            //
        }

        _connections.TryRemove(connection.ConnectionId, out _);
        logger.LogWarning("断开连接啦，远程地址-{RemoteEndPoint}-{LocalEndPoint}-连接数-{Count}", connection.RemoteEndPoint,
            connection.LocalEndPoint, _connections.Count);
    }
}
