using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using SuperSocket.Connection;
using SuperSocket.Kestrel.Connection;
using SuperSocket.ProtoBase;
using WebApplicationTarget.Middlewares;

namespace WebApplicationTarget;

internal sealed class TelnetConnectionHandler(
    IConnectionContainer container,
    ILogger<TelnetConnectionHandler> logger)
    : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext context)
    {
        var transferFormat = context.Features.Get<ITransferFormatFeature>();
        if (transferFormat != null)
        {
            container.Add(context);
            transferFormat.ActiveFormat = TransferFormat.Binary;
        }

        var pipeConnection = new KestrelPipeConnection(context, new ConnectionOptions
        {
            Logger = logger,
        });

        try
        {
            await foreach (var pack in pipeConnection.RunAsync(new LinePipelineFilter()))
            {
                logger.LogInformation($"Received message: {pack}");
                await pipeConnection.SendAsync("ddffdgfgf\r\n"u8.ToArray(), CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"发生异常-{context.ConnectionId}{context.RemoteEndPoint}");
        }
        finally
        {
            if (transferFormat != null)
                container.Remove(context);

            await context.DisposeAsync();
        }
    }
}
