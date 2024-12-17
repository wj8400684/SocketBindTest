using Kestrel.Socket.Server.Middlewares;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Options;
using SuperSocket.Connection;
using SuperSocket.Kestrel;
using SuperSocket.ProtoBase;

namespace Kestrel.Socket.Server.ConnectionHandlers;

internal sealed class TelnetConnectionHandler(
    ILogger<TelnetConnectionHandler> logger)
    : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        var pipeConnection = new KestrelPipeConnection(connection, new ConnectionOptions
        {
            Logger = logger,
        });

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
    }
}
