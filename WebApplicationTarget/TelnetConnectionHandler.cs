using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using SuperSocket.Connection;
using SuperSocket.Kestrel;
using SuperSocket.ProtoBase;

namespace WebApplicationTarget;

internal sealed class TelnetConnectionHandler(
    ILogger<TelnetConnectionHandler> logger)
    : ConnectionHandler
{
    public override async Task OnConnectedAsync(ConnectionContext context)
    {
        var transferFormat = context.Features.Get<ITransferFormatFeature>();
        if (transferFormat != null)
            transferFormat.ActiveFormat = TransferFormat.Binary;
        
        var pipeConnection = new KestrelPipeConnection(context, new ConnectionOptions
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
            logger.LogError(e, $"发生异常-{context.ConnectionId}{context.RemoteEndPoint}");
        }
    }
}
