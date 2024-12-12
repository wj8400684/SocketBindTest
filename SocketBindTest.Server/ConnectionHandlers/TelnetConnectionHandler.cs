using System.Collections.Concurrent;
using Microsoft.AspNetCore.Connections;
using SuperSocket.Connection;
using SuperSocket.Kestrel;
using SuperSocket.ProtoBase;

namespace SocketBindTest.Server;

internal sealed class TelnetConnectionHandler(ILogger<TelnetConnectionHandler> logger) : ConnectionHandler
{
    private readonly ConcurrentDictionary<string, ConnectionContext> _connections = new();

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var pipeConnection = new KestrelPipeConnection(connection, new ConnectionOptions
        {
            Logger = logger,
        });

        _connections.TryAdd(connection.ConnectionId, connection);

        logger.LogInformation($"新连接-{connection.ConnectionId}{connection.RemoteEndPoint}->{_connections.Count}");

        try
        {
            await foreach (var pack in pipeConnection.RunAsync(new LinePipelineFilter()))
            {
                Console.WriteLine(pack.Text);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"发生异常-{connection.ConnectionId}{connection.RemoteEndPoint}");
        }
        finally
        {
            logger.LogInformation($"断开连接-{connection.ConnectionId}{connection.RemoteEndPoint}");
            _connections.TryRemove(connection.ConnectionId, out _);
        }
    }
}