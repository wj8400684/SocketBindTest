using Microsoft.AspNetCore.SignalR;

namespace Kestrel.Socket.Server.Chat;

public interface IChatConnectionContainer
{
    bool Add(HubCallerContext connection);
    
    bool Remove(HubCallerContext connection);

    HubCallerContext? GetConnectionById(string connectionId);

    int GetConnectionCount();

    IEnumerable<HubCallerContext> GetConnections(Predicate<HubCallerContext>? criteria = null);
}