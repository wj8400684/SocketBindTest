using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Connections;

namespace SocketBindTest.Server;

internal sealed class TelnetConnectionHandler(ILogger<TelnetConnectionHandler> logger) : ConnectionHandler
{
    private readonly DnsEndPoint _remoteEndPoint = new("www.ip138.com", 443, AddressFamily.InterNetwork);

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        if (connection.LocalEndPoint is not IPEndPoint localEndPoint)
        {
            logger.LogWarning("ip不正确");
            return;
        }

        logger.LogInformation("新连接，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
            connection.LocalEndPoint);

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.ExclusiveAddressUse = false;
            socket.Bind(localEndPoint);
        }
        catch (Exception e)
        {
            logger.LogError(e, "绑定ip失败，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
            return;
        }

        await socket.ConnectAsync(_remoteEndPoint);

        var stream = new NetworkStream(socket, true);

        try
        {
            await connection.Transport.Input.CopyToAsync(stream);
            await stream.CopyToAsync(connection.Transport.Output);
        }
        catch (Exception e)
        {
            logger.LogError(e, "ip不正确");
        }
        finally
        {
            logger.LogWarning("断开连接啦，远程地址-{RemoteEndPoint}-{LocalEndPoint}", connection.RemoteEndPoint,
                connection.LocalEndPoint);
        }
    }
}