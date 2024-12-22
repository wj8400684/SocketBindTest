using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using WebApplicationTarget.Middlewares;

namespace WebApplicationTarget;

internal sealed class WebSocketConnectionHandler(
    IConnectionContainer container,
    ILogger<TelnetConnectionHandler> logger)
    : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext context)
    {
        var transferFormat = context.Features.Get<ITransferFormatFeature>();
        if (transferFormat != null)
            transferFormat.ActiveFormat = TransferFormat.Binary;

        container.Add(context);

        logger.LogInformation($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：新连接-{container.GetConnectionCount()}");

        await using var connection = new WebSocketConnection(context);

        try
        {
            await foreach (var pack in connection.RunAsync())
            {
                logger.LogInformation($"Received message: {pack}");
                await connection.SendAsync("ddffdgfgf\r\n"u8.ToArray(), CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"发生异常-{context.ConnectionId}{context.RemoteEndPoint}");
        }
        finally
        {
            container.Remove(context);
            logger.LogInformation($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：客户端断开连接");
        }
    }
}
