using Microsoft.AspNetCore.SignalR;

namespace Kestrel.Socket.Server.Chat;

public sealed class ChatHub(IChatConnectionContainer container, ILogger<ChatHub> logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        container.Add(Context);
        
        logger.LogInformation("a new chat connected [{ConnectionId}]- count [{count}]", Context.ConnectionId,
            container.GetConnectionCount());
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        container.Remove(Context);
        
        logger.LogInformation("chat closed [{ConnectionId}]", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}