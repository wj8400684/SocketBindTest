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
    private readonly DnsEndPoint _target = new("159.75.132.21", 9000, AddressFamily.InterNetwork);
    
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        logger.LogInformation("新连接，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

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

        using var socket = new System.Net.Sockets.Socket(_target.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

#if IsWindows 
        try
        {
            socket.ExclusiveAddressUse = false;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.IpTimeToLive, true);
        }
        catch (Exception ex)
        {
            throw new Exception($"设置socket相关参数发生异常-{state.BindEndPoint}", ex);
        }
#endif
        
        var address = localEndPoint.Address.MapToIPv4();

        try
        {
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
