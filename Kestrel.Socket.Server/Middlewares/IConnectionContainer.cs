using Microsoft.AspNetCore.Connections;

namespace Kestrel.Socket.Server.Middlewares;

public interface IConnectionContainer
{
    bool Add(ConnectionContext connection);
    
    bool Remove(ConnectionContext connection);

    ConnectionContext? GetConnectionById(string connectionId);

    int GetConnectionCount();

    IEnumerable<ConnectionContext> GetConnections(Predicate<ConnectionContext>? criteria = null);
}