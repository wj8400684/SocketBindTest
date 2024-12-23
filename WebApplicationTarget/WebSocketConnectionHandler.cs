using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using WebApplicationTarget.Middlewares;

namespace WebApplicationTarget;

internal sealed class WebSocketConnectionHandler(
    IConnectionContainer container,
    ILogger<TelnetConnectionHandler> logger)
    : ConnectionHandler, IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public override async Task OnConnectedAsync(ConnectionContext context)
    {
        var transferFormat = context.Features.Get<ITransferFormatFeature>();
        if (transferFormat != null)
            transferFormat.ActiveFormat = TransferFormat.Binary;

        container.Add(context);

        logger.LogDebug($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：新连接-{container.GetConnectionCount()}");

        await using var connection = new WebSocketConnection(context);

        try
        {
            await foreach (var pack in connection.RunAsync(_cancellationTokenSource.Token))
            {
                logger.LogDebug($"Received message: {pack}");
                await connection.SendAsync("ddffdgfgf\r\n"u8.ToArray(), _cancellationTokenSource.Token);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"发生异常-{context.ConnectionId}{context.RemoteEndPoint}");
        }
        finally
        {
            container.Remove(context);
            logger.LogDebug($"[{context.RemoteEndPoint}]-[{context.ConnectionId}]：客户端断开连接");
        }
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}